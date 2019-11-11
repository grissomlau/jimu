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
            if (eventData.Data.PayLoad == null)
                eventData.Data.PayLoad = new JimuPayload();
            var operation = $"RPC: {eventData.Data.Service.ServiceDescriptor.RoutePath}";
            var context = _tracingContext.CreateExitSegmentContext(operation, eventData.Data.ServiceAddress.ToString(), new JimuClientCarrierHeaderCollection(eventData.Data.PayLoad));

            if (eventData.Data.Service != null)
            {
                context.Span.AddTag("Service", eventData.Data.Service.ServiceDescriptor.Id);
                context.Span.AddTag("AllowAnonymous", eventData.Data.Service.ServiceDescriptor.AllowAnonymous);
            }
            context.Span.AddLog(LogEvent.Event("Call start"));

        }

        [DiagnosticName(DiagnosticClientEventType.RpcExecuteAfter)]
        public void AfterRPCExecute([Object] RPCExecuteAfterEventData eventData)
        {
            //_tracingContext.Release(_entrySegmentContextAccessor.Context);
            var context = _exitSegmentContextAccessor.Context;
            if (context == null) return;
            context.Span.AddTag("HasError", eventData.ResultData.HasError);
            var logEvent = $"Call finished";
            var logMsg = $"HasError: {eventData.ResultData.HasError}";
            if (eventData.ResultData.HasError)
            {
                logMsg += $",ErrorCode: {eventData.ResultData.ErrorCode}";
                logMsg += $",ErrorMsg: {eventData.ResultData.ErrorMsg}";
                logMsg += $",Exception: {eventData.ResultData.ExceptionMessage}";
            }
            context.Span.AddLog(LogEvent.Event(logEvent), LogEvent.Message(logMsg));
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
