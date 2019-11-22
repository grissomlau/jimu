using SkyApm;
using System;
using Jimu.Client.Diagnostic;
using SkyApm.Tracing;
using SkyApm.Diagnostics;
using System.Collections.Generic;
using SkyApm.Tracing.Segments;
using System.Linq;
using SkyWalking.NetworkProtocol;
using Jimu.Client.Diagnostic.EventData;
using System.Text;
using SkyApm.Common;

namespace Jimu.Client.Diagnostic.Skywalking
{
    public class JimuClientDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName => DiagnosticClientEventType.ListenerName;

        private readonly ITracingContext _tracingContext;
        private readonly IExitSegmentContextAccessor _exitSegmentContextAccessor;

        public JimuClientDiagnosticProcessor(ITracingContext tracingContext
            , IExitSegmentContextAccessor exitSegmentContextAccessor
            )
        {
            _tracingContext = tracingContext;
            _exitSegmentContextAccessor = exitSegmentContextAccessor;
        }

        [DiagnosticName(DiagnosticClientEventType.RpcExecuteBefore)]
        public void BeforeRPCExecute([Object] RPCExecuteBeforeEventData eventData)
        {
            if (eventData.Data.Payload == null)
                eventData.Data.Payload = new JimuPayload();
            var operation = $"client-rpc: {eventData.Data.Service.ServiceDescriptor.RoutePath}";
            var context = _tracingContext.CreateExitSegmentContext(operation, eventData.Data.ServiceAddress.ToString(), new JimuClientCarrierHeaderCollection(eventData.Data.Payload));

            if (eventData.Data.Service != null)
            {
                context.Span.AddTag("service", eventData.Data.Service.ServiceDescriptor.Id);
                context.Span.AddTag("allowAnonymous", eventData.Data.Service.ServiceDescriptor.AllowAnonymous);
            }
            StringBuilder sbLog = new StringBuilder();

            if (eventData.Data?.Paras != null)
            {
                sbLog.AppendLine($"parameters =>");
                foreach (var para in eventData.Data.Paras)
                {
                    if (para.Value is List<JimuFile>)
                    {
                        foreach (var file in para.Value as List<JimuFile>)
                        {
                            sbLog.AppendLine($"{para.Key}: {file.FileName}");
                        }
                    }
                    else
                    {
                        sbLog.AppendLine($"{para.Key}: {para.Value}");
                    }
                }
            }

            sbLog.AppendLine($"token =>");
            sbLog.AppendLine($"{eventData.Data?.Token}");
            if (eventData.Data.Payload?.Items != null)
            {
                sbLog.AppendLine($"payload =>");
                foreach (var item in eventData.Data.Payload.Items)
                {
                    sbLog.AppendLine($"{item.Key}: {item.Value}");
                }
            }
            else
            {
                sbLog.AppendLine($"payload is null");
            }
            context.Span.AddLog(LogEvent.Event("start"), LogEvent.Message(sbLog.ToString()));

        }

        [DiagnosticName(DiagnosticClientEventType.RpcExecuteAfter)]
        public void AfterRPCExecute([Object] RPCExecuteAfterEventData eventData)
        {
            //_tracingContext.Release(_entrySegmentContextAccessor.Context);
            var context = _exitSegmentContextAccessor.Context;
            if (context == null) return;
            context.Span.AddTag("hasError", eventData.ResultData.HasError);
            var logEvent = $"end";
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendLine($"HasError: {eventData.ResultData.HasError}");
            if (eventData.ResultData.HasError)
            {
                sbLog.AppendLine($"ErrorCode: {eventData.ResultData.ErrorCode}");
                sbLog.AppendLine($"ErrorMsg: {eventData.ResultData.ErrorMsg}");
                sbLog.AppendLine($"Exception: {eventData.ResultData.ExceptionMessage}");
            }
            context.Span.AddLog(LogEvent.Event(logEvent), LogEvent.Message(sbLog.ToString()));
            _tracingContext.Release(_exitSegmentContextAccessor.Context);
        }

        [DiagnosticName(DiagnosticClientEventType.RpcExecuteError)]
        public void ErrorRPCExecute([Object] RPCExecuteErrorEventData eventData)
        {
            var context = _exitSegmentContextAccessor.Context;
            if (context != null)
            {
                context.Span.ErrorOccurred(eventData.Ex);
                _tracingContext.Release(context);
            }
        }
    }
}
