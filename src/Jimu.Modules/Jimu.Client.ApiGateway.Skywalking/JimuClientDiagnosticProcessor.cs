using SkyApm;
using System;
using Jimu.Client.Diagnostics;
using SkyApm.Tracing;
using SkyApm.Diagnostics;
using System.Collections.Generic;
using SkyApm.Tracing.Segments;
using System.Linq;

namespace Jimu.Client.ApiGateway.Skywalking
{
    public class JimuClientDiagnosticProcessor : ITracingDiagnosticProcessor
    {
        public string ListenerName => JimuClientDiagnosticListenerExtensions.JIMU_CLIENT_DIAGNOSTIC_LISTENER;

        private readonly ITracingContext _tracingContext;
        private readonly IExitSegmentContextAccessor _exitSegmentContextAccessor;

        public JimuClientDiagnosticProcessor(ITracingContext tracingContext
            , IExitSegmentContextAccessor exitSegmentContextAccessor
            )
        {
            _tracingContext = tracingContext;
            _exitSegmentContextAccessor = exitSegmentContextAccessor;
        }

        [DiagnosticName(JimuClientDiagnosticListenerExtensions.JIMU_CLIENT_BEFORE_PRC_EXECUTE)]
        public void BeforeRPCExecute([Object] RPCExecuteBeforeEventData eventData)
        {
            var context = _tracingContext.CreateExitSegmentContext(eventData.Operation, eventData.Context.ServiceAddress.ToString());

            if (eventData.Context.Service != null)
            {
                context.Span.AddTag("Service", eventData.Context.Service.ServiceDescriptor.Id);
                context.Span.AddTag("AllowAnonymous", eventData.Context.Service.ServiceDescriptor.AllowAnonymous);
            }
            context.Span.AddLog(new LogEvent("event", "Invoke starting"));

        }

        [DiagnosticName(JimuClientDiagnosticListenerExtensions.JIMU_CLIENT_AFTER_PRC_EXECUTE)]
        public void AfterRPCExecute([Object] RPCExecuteAfterEventData eventData)
        {
            //_tracingContext.Release(_entrySegmentContextAccessor.Context);
            var context = _exitSegmentContextAccessor.Context;
            context.Span.AddTag("HasError", eventData.ResultData.HasError);
            var logMsg = $"Invoke finished, HasError: {eventData.ResultData.HasError}";
            if (eventData.ResultData.HasError)
            {
                logMsg += $",ErrorCode: {eventData.ResultData.ErrorCode}";
                logMsg += $",ErrorMsg: {eventData.ResultData.ErrorMsg}";
                logMsg += $",Exception: {eventData.ResultData.ExceptionMessage}";
            }
            context.Span.AddLog(new LogEvent("event", logMsg));
            //context.Span.AddTag("ResultType", eventData.ResultData.ResultType);
            _tracingContext.Release(_exitSegmentContextAccessor.Context);
        }

        [DiagnosticName(JimuClientDiagnosticListenerExtensions.JIMU_CLIENT_ERROR_PRC_EXECUTE)]
        public void ErrorRPCExecute([Object] RPCExecuteErrorEventData eventData)
        {
            //var context = _entrySegmentContextAccessor.Context;
            var context = _exitSegmentContextAccessor.Context;
            if (context != null)
            {
                context.Span.ErrorOccurred(eventData.Ex);
                _tracingContext.Release(context);
            }
        }
    }
}
