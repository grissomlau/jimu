using Jimu.Server.Diagnostic.EventData.LocalMethod;
using Jimu.Server.Diagnostic.EventData.ServiceInvoke;
using SkyApm;
using SkyApm.Diagnostics;
using SkyApm.Tracing;
using SkyApm.Tracing.Segments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Diagnostic.Skywalking
{
    public class JimuServerDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName => DiagnosticServerEventType.ListenerName;


        private readonly ITracingContext _tracingContext;
        private readonly IEntrySegmentContextAccessor _entrySegmentContextAccessor;
        private readonly ILocalSegmentContextAccessor _localSegmentContextAccessor;

        public JimuServerDiagnosticProcessor(ITracingContext tracingContext
            , IEntrySegmentContextAccessor entrySegmentContextAccessor
            , ILocalSegmentContextAccessor localSegmentContextAccessor
            )
        {
            _tracingContext = tracingContext;
            _entrySegmentContextAccessor = entrySegmentContextAccessor;
            _localSegmentContextAccessor = localSegmentContextAccessor;
        }

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeBefore)]
        public void BeforeServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            var operation = $"server-invoke: {eventData.Operation}";
            var context = _tracingContext.CreateEntrySegmentContext(operation, new JimuServerCarrierHeaderCollection(eventData.Data.RemoteInvokeMessage.Payload));
            context.Span.Peer = eventData.Data.Address.ToString();

            if (eventData.Data.ServiceEntry != null)
            {
                context.Span.AddTag("service", eventData.Data.ServiceEntry.Descriptor.Id);
                context.Span.AddTag("allowAnonymous", eventData.Data.ServiceEntry.Descriptor.AllowAnonymous);
            }

            StringBuilder sbLog = new StringBuilder();
            if (eventData.Data.RemoteInvokeMessage?.Parameters != null)
            {
                sbLog.AppendLine($"parameters =>");
                foreach (var para in eventData.Data.RemoteInvokeMessage.Parameters)
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
            sbLog.AppendLine($"{eventData.Data.RemoteInvokeMessage?.Token}");
            if (eventData.Data.RemoteInvokeMessage?.Payload?.Items != null)
            {
                sbLog.AppendLine($"payLoad =>");
                foreach (var item in eventData.Data.RemoteInvokeMessage.Payload.Items)
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

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeAfter)]
        public void AfterServiceInvoke([Object] ServiceInvokeAfterEventData eventData)
        {
            var context = _entrySegmentContextAccessor.Context;
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
            _tracingContext.Release(context);
        }

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeError)]
        public void ErrorServiceInvoke([Object] ServiceInvokeErrorEventData eventData)
        {
            var context = _entrySegmentContextAccessor.Context;
            if (context != null)
            {
                context.Span.ErrorOccurred(eventData.Ex);
                _tracingContext.Release(context);
            }
        }

        [DiagnosticName(DiagnosticServerEventType.LocalMethodInvokeBefore)]
        public void BeforeLocalMethodInvoke([Object] LocalMethodInvokeBeforeEventData eventData)
        {
            var operation = $"server-local: {eventData.Operation}";
            var context = _tracingContext.CreateLocalSegmentContext(operation);
            context.Span.AddLog(LogEvent.Event("start"), LogEvent.Message(eventData.Data));
        }

        [DiagnosticName(DiagnosticServerEventType.LocalMethodInvokeAfter)]
        public void AfterLocalMethodInvoke([Object] LocalMethodInvokeAfterEventData eventData)
        {
            var context = _localSegmentContextAccessor.Context;
            if (context == null) return;
            context.Span.AddLog(LogEvent.Event("end"), LogEvent.Message(eventData.Data));
            _tracingContext.Release(context);
        }

        [DiagnosticName(DiagnosticServerEventType.LocalMethodInvokeError)]
        public void ErrorLocalMethodInvoke([Object] LocalMethodInvokeErrorEventData eventData)
        {
            var context = _localSegmentContextAccessor.Context;
            if (context != null)
            {
                context.Span.ErrorOccurred(eventData.Ex);
                _tracingContext.Release(context);
            }
        }
    }
}
