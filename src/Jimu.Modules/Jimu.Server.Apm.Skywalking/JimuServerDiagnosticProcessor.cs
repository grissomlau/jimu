using Jimu.Server.APM;
using Jimu.Server.APM.EventData.ServiceInvoke;
using SkyApm;
using SkyApm.Diagnostics;
using SkyApm.Tracing;
using System;

namespace Jimu.Server.Apm.Skywalking
{
    public class JimuServerDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName => ApmServerEventType.ListenerName;


        private readonly ITracingContext _tracingContext;
        private readonly IEntrySegmentContextAccessor _entrySegmentContextAccessor;

        public JimuServerDiagnosticProcessor(ITracingContext tracingContext
            , IEntrySegmentContextAccessor entrySegmentContextAccessor
            )
        {
            _tracingContext = tracingContext;
            _entrySegmentContextAccessor = entrySegmentContextAccessor;
        }

        [DiagnosticName(ApmServerEventType.ServiceInvokeBefore)]
        public void BeforeServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            var context = _tracingContext.CreateEntrySegmentContext(eventData.Operation, new JimuServerCarrierHeaderCollection(eventData.Data.RemoteInvokeMessage.Payload));
            context.Span.AddTag("name", "fuck");
        }

        [DiagnosticName(ApmServerEventType.ServiceInvokeAfter)]
        public void AfterServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            var context = _entrySegmentContextAccessor.Context;
            if (context == null) return;
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
