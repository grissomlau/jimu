using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jimu.Server.Transport.Http
{
    public class HttpMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Stack<Func<RequestDel, RequestDel>> _middlewares;
        private readonly IServiceEntryContainer _serviceEntryContainer;
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;
        private JimuTransportMsg _message;
        private readonly ITypeConvertProvider _typeConvert;
        public HttpMiddleware(RequestDelegate next, Stack<Func<RequestDel, RequestDel>> middlewares, IServiceEntryContainer serviceEntryContainer, ILogger logger, ISerializer serializer, ITypeConvertProvider typeConvert)
        {
            _next = next;
            _middlewares = middlewares;
            _serviceEntryContainer = serviceEntryContainer;
            _logger = logger;
            _serializer = serializer;
            _typeConvert = typeConvert;
        }

        public async Task Invoke(HttpContext context)
        {
            using (var sr = new StreamReader(context.Request.Body))
            {
                var body = sr.ReadToEnd();
                _logger.Info($"received msg is: {body}");
                _message = (JimuTransportMsg)_typeConvert.Convert(body, typeof(JimuTransportMsg));
            }

            _logger.Info($"begin handling msg: {_message.Id}");
            IResponse response = new HttpResponse(context.Response, _serializer, _logger);
            var thisContext = new RemoteCallerContext(_message, _serviceEntryContainer, response, _logger);
            var lastInvoke = new RequestDel(async ctx =>
            {
                JimuRemoteCallResultData resultMessage = new JimuRemoteCallResultData();

                if (ctx.ServiceEntry == null)
                {
                    resultMessage.ExceptionMessage = $"can not find service {ctx.RemoteInvokeMessage.ServiceId}";
                    await response.WriteAsync(_message.Id, resultMessage);
                }
                else if (ctx.ServiceEntry.Descriptor.WaitExecution)
                {
                    await LocalServiceExecuteAsync(ctx.ServiceEntry, ctx.RemoteInvokeMessage, resultMessage);
                    await response.WriteAsync(_message.Id, resultMessage);
                }
                else
                {
                    await response.WriteAsync(_message.Id, resultMessage);
                    await Task.Factory.StartNew(async () =>
                    {
                        await LocalServiceExecuteAsync(ctx.ServiceEntry, ctx.RemoteInvokeMessage, resultMessage);
                    });
                }
            });


            foreach (var middleware in _middlewares)
            {
                lastInvoke = middleware(lastInvoke);
            }

            await lastInvoke(thisContext);
            //await _next(context);

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
                    resultMessage.ResultType = serviceEntry.Descriptor.ReturnType;

                }
            }
            catch (Exception ex)
            {
                _logger.Error("throw exception when excuting local service: " + serviceEntry.Descriptor.Id, ex);
                resultMessage.ExceptionMessage = ex.ToStackTraceString();
            }
        }

    }
}
