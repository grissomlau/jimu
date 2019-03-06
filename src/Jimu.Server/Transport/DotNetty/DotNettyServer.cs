using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Jimu.Server.Transport.DotNetty
{
    public class DotNettyServer : IServer
    {
        private readonly List<JimuServiceRoute> _serviceRoutes = new List<JimuServiceRoute>();
        private readonly IServiceEntryContainer _serviceEntryContainer;
        private readonly DotNettyAddress _address;
        private readonly ILogger _logger;
        private IChannel _channel;

        private readonly Stack<Func<RequestDel, RequestDel>> _middlewares;


        public DotNettyServer(DotNettyAddress address, IServiceEntryContainer serviceEntryContainer, ILogger logger)
        {
            _serviceEntryContainer = serviceEntryContainer;
            _address = address;
            _logger = logger;
            _middlewares = new Stack<Func<RequestDel, RequestDel>>();
        }
        public List<JimuServiceRoute> GetServiceRoutes()
        {
            if (!_serviceRoutes.Any())
            {
                var serviceEntries = _serviceEntryContainer.GetServiceEntry();
                serviceEntries.ForEach(entry =>
                {
                    var serviceRoute = new JimuServiceRoute
                    {
                        Address = new List<JimuAddress> {
                             _address
                            },
                        ServiceDescriptor = entry.Descriptor
                    };
                    _serviceRoutes.Add(serviceRoute);
                });
            }

            return _serviceRoutes;
        }

        private async Task OnReceived(IChannelHandlerContext channel, JimuTransportMsg message)
        {
            _logger.Debug($"begin handling msg: {message.Id}");
            //TaskCompletionSource<TransportMessage> task;
            if (message.ContentType == typeof(JimuRemoteCallData).FullName)
            {
                IResponse response = new DotNettyResponse(channel, _logger);
                var thisContext = new RemoteCallerContext(message, _serviceEntryContainer, response, _logger);

                var lastInvoke = new RequestDel(async context =>
                {
                    JimuRemoteCallResultData resultMessage = new JimuRemoteCallResultData();
                    if (context.ServiceEntry == null)
                    {
                        resultMessage.ExceptionMessage = $"can not find service {context.RemoteInvokeMessage.ServiceId}";
                        await response.WriteAsync(message.Id, resultMessage);
                    }
                    else if (context.ServiceEntry.Descriptor.WaitExecution)
                    {
                        await LocalServiceExecuteAsync(context.ServiceEntry, context.RemoteInvokeMessage, resultMessage);
                        await response.WriteAsync(message.Id, resultMessage);
                    }
                    else
                    {
                        await response.WriteAsync(message.Id, resultMessage);
                        await Task.Factory.StartNew(async () =>
                        {
                            await LocalServiceExecuteAsync(context.ServiceEntry, context.RemoteInvokeMessage, resultMessage);
                        });
                    }
                });

                foreach (var middleware in _middlewares)
                {
                    lastInvoke = middleware(lastInvoke);
                }
                await lastInvoke(thisContext);

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
            _logger.Info($"start server: {_address.Code}");
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
            _channel = await bootstrap.BindAsync(_address.CreateEndPoint());

            _logger.Info($"server start successfuly, address is： {_address.Code}");
        }

        private async Task InvokeMiddleware(RequestDel next, RemoteCallerContext context)
        {
            await next.Invoke(context);
        }

        public IServer Use(Func<RequestDel, RequestDel> middleware)
        {
            _middlewares.Push(middleware);
            return this;
        }
    }
}