using Jimu.Server.Diagnostic;
using Jimu.Server.Diagnostic.EventData.ServiceInvoke;
using SkyApm;
using SkyApm.Diagnostics;
using SkyApm.Tracing;
using SkyApm.Tracing.Segments;
using System;

namespace Jimu.Server.Diagnostic.Skywalking
{
    public class JimuServerDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName => DiagnosticServerEventType.ListenerName;


        private readonly ITracingContext _tracingContext;
        private readonly IEntrySegmentContextAccessor _entrySegmentContextAccessor;

        public JimuServerDiagnosticProcessor(ITracingContext tracingContext
            , IEntrySegmentContextAccessor entrySegmentContextAccessor
            )
        {
            _tracingContext = tracingContext;
            _entrySegmentContextAccessor = entrySegmentContextAccessor;
        }

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeBefore)]
        public void BeforeServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            var context = _tracingContext.CreateEntrySegmentContext(eventData.Operation, new JimuServerCarrierHeaderCollection(eventData.Data.RemoteInvokeMessage.Payload));
            context.Span.Peer = eventData.Data.Address.ToString();

            if (eventData.Data.ServiceEntry != null)
            {
                context.Span.AddTag("Service", eventData.Data.ServiceEntry.Descriptor.Id);
                context.Span.AddTag("AllowAnonymous", eventData.Data.ServiceEntry.Descriptor.AllowAnonymous);
            }
            context.Span.AddLog(LogEvent.Event("Invoke start"));
        }

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeAfter)]
        public void AfterServiceInvoke([Object] ServiceInvokeAfterEventData eventData)
        {
            var context = _entrySegmentContextAccessor.Context;
            if (context == null) return;
            context.Span.AddTag("HasError", eventData.ResultData.HasError);

            var logEvent = $"Invoke finished";
            var logMsg = $"HasError: {eventData.ResultData.HasError}";
            if (eventData.ResultData.HasError)
            {
                logMsg += $",ErrorCode: {eventData.ResultData.ErrorCode}";
                logMsg += $",ErrorMsg: {eventData.ResultData.ErrorMsg}";
                logMsg += $",Exception: {eventData.ResultData.ExceptionMessage}";
            }
            context.Span.AddLog(LogEvent.Event(logEvent), LogEvent.Message(logMsg));
            _tracingContext.Release(context);
        }

        public void ErrorServiceInvoke([Object] ServiceInvokeErrorEventData eventData)
        {
            var context = _entrySegmentContextAccessor.Context;
            if (context != null)
            {
                context.Span.ErrorOccurred(eventData.Ex);
                _tracingContext.Release(context);
            }
        }
    }
}
