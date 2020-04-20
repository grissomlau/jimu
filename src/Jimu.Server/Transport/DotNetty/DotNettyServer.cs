using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Jimu.Diagnostic;
using Jimu.Common;
using Jimu.Logger;
using Jimu.Server.Diagnostic;
using Jimu.Server.ServiceContainer;
using Jimu.Server.Transport.DotNetty.Adapter;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Jimu.Server.Transport.DotNetty
{
    public class DotNettyServer : IServer
    {
        //private readonly List<JimuServiceRoute> _serviceRoutes = new List<JimuServiceRoute>();
        private readonly IServiceEntryContainer _serviceEntryContainer;
        private readonly JimuAddress _serviceInvokeAddress;
        private readonly ILogger _logger;
        private IChannel _channel;
        private readonly string _serverIp;
        private readonly int _serverPort;

        private readonly Stack<Func<RequestDel, RequestDel>> _middlewares;

        private readonly IJimuDiagnostic _jimuApm;



        public DotNettyServer(string serverIp, int serverPort, JimuAddress serviceInvokeAddress, IServiceEntryContainer serviceEntryContainer, IJimuDiagnostic jimuApm, ILogger logger)
        {
            _serviceEntryContainer = serviceEntryContainer;
            _serviceInvokeAddress = serviceInvokeAddress;
            _logger = logger;
            _middlewares = new Stack<Func<RequestDel, RequestDel>>();
            _serverIp = serverIp;
            _serverPort = serverPort;
            _jimuApm = jimuApm;
        }
        public List<JimuServiceRoute> GetServiceRoutes()
        {
            List<JimuServiceRoute> routes = new List<JimuServiceRoute>();
            var serviceEntries = _serviceEntryContainer.GetServiceEntry();
            serviceEntries.ForEach(entry =>
            {
                var serviceRoute = new JimuServiceRoute
                {
                    Address = new List<JimuAddress> {
                             _serviceInvokeAddress
                        },
                    ServiceDescriptor = entry.Descriptor
                };
                routes.Add(serviceRoute);
            });

            return routes;
        }

        private async Task OnReceived(IChannelHandlerContext channel, JimuTransportMsg message)
        {
            _logger.Debug($"begin handling msg: {message.Id}");
            //TaskCompletionSource<TransportMessage> task;
            if (message.ContentType == typeof(JimuRemoteCallData).FullName)
            {
                IResponse response = new DotNettyResponse(channel, _logger);
                var thisContext = new ServiceInvokerContext(message, _serviceEntryContainer, response, _logger, _serviceInvokeAddress);
                Guid operationId = Guid.Empty;

                var lastInvoke = new RequestDel(async context =>
                {
                    JimuRemoteCallResultData resultMessage = new JimuRemoteCallResultData();
                    if (context.ServiceEntry == null)
                    {
                        resultMessage.ExceptionMessage = $"can not find service {context.RemoteInvokeMessage.ServiceId}";
                        _jimuApm.WriteServiceInvokeAfter(operationId, thisContext, resultMessage);
                        await response.WriteAsync(message.Id, resultMessage);
                    }
                    else if (context.ServiceEntry.Descriptor.WaitExecution)
                    {
                        await LocalServiceExecuteAsync(context.ServiceEntry, context.RemoteInvokeMessage, resultMessage);
                        _jimuApm.WriteServiceInvokeAfter(operationId, thisContext, resultMessage);
                        await response.WriteAsync(message.Id, resultMessage);
                    }
                    else
                    {
                        await response.WriteAsync(message.Id, resultMessage);
                        await Task.Factory.StartNew(async () =>
                        {
                            await LocalServiceExecuteAsync(context.ServiceEntry, context.RemoteInvokeMessage, resultMessage);
                            _jimuApm.WriteServiceInvokeAfter(operationId, thisContext, resultMessage);
                        });
                    }
                });

                foreach (var middleware in _middlewares)
                {
                    lastInvoke = middleware(lastInvoke);
                }
                try
                {
                    _jimuApm.WriteServiceInvokeBefore(thisContext);
                    await lastInvoke(thisContext);
                }
                catch (Exception ex)
                {
                    JimuRemoteCallResultData resultMessage = new JimuRemoteCallResultData();
                    resultMessage.ErrorCode = "500";
                    resultMessage.ExceptionMessage = ex.ToStackTraceString();
                    _logger.Error("throw exception when excuting local service: \r\n " + JimuHelper.Serialize<string>(message), ex);
                    await response.WriteAsync(message.Id, resultMessage);
                }

            }
            else
            {
                _logger.Debug($"msg: {message.Id}, message type is not an  JimuRemoteCallData.");
            }
        }

        private async Task LocalServiceExecuteAsync(JimuServiceEntry serviceEntry, JimuRemoteCallData invokeMessage, JimuRemoteCallResultData resultMessage)
        {
            try
            {
                var cancelTokenSource = new CancellationTokenSource();
                //wait OnAuthorization(serviceEntry, cancelTokenSource ,,,,)
                if (!cancelTokenSource.IsCancellationRequested)
                {
                    var result = await serviceEntry.Func(invokeMessage.Parameters, invokeMessage.Payload);
                    var task = result as Task;
                    if (task == null)
                    {
                        resultMessage.Result = result;
                    }
                    else
                    {
                        task.Wait(cancelTokenSource.Token);
                        var taskType = task.GetType().GetTypeInfo();
                        if (taskType.IsGenericType)
                        {
                            resultMessage.Result = taskType.GetProperty("Result")?.GetValue(task);
                        }
                    }
                    resultMessage.ResultType = serviceEntry.Descriptor.ReturnDesc;

                }
            }
            catch (Exception ex)
            {
                _logger.Error("throw exception when excuting local service: " + serviceEntry.Descriptor.Id, ex);
                resultMessage.ExceptionMessage = ex.ToStackTraceString();
            }
        }



        public async Task StartAsync()
        {
            _logger.Info($"start server: {_serverIp}:{_serverPort}");
            var bossGroup = new MultithreadEventLoopGroup();
            var workerGroup = new MultithreadEventLoopGroup(4);
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                //.Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(new ReadServerMessageChannelHandlerAdapter(_logger));
                    pipeline.AddLast(new ServerHandlerChannelHandlerAdapter(async (context, message) =>
                    {
                        await OnReceived(context, message);
                    }, _logger));
                }));

            //var endpoint = new IPEndPoint(IPAddress.Parse(this.addre), this._port);
            //_channel = await bootstrap.BindAsync(_address.CreateEndPoint()); // bind with ip not support in docker, will not connected
            if (_serverIp != "0.0.0.0")
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(this._serverIp), this._serverPort);
                _channel = await bootstrap.BindAsync(endpoint); // bind with ip not support in docker, will not connected
                _logger.Info($"server start successfully, address is： {_serverIp}:{_serverPort}");
            }
            else
            {
                _channel = await bootstrap.BindAsync(_serverPort);
                _logger.Info($"server start successfully, address is： {JimuHelper.GetLocalIPAddress()}:{_serverPort}");
            }

        }

        private async Task InvokeMiddleware(RequestDel next, ServiceInvokerContext context)
        {
            await next.Invoke(context);
        }

        public IServer Use(Func<RequestDel, RequestDel> middleware)
        {
            _middlewares.Push(middleware);
            return this;
        }

        public IPEndPoint GetServerAddress()
        {
            if (_serverIp == "0.0.0.0")
            {
                return new IPEndPoint(IPAddress.Parse(JimuHelper.GetLocalIPAddress()), _serverPort);
            }
            return new IPEndPoint(IPAddress.Parse(_serverIp), _serverPort);
        }
    }
}