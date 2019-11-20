using Jimu.Server.Diagnostic;
using Jimu.Server.Diagnostic.EventData.ServiceInvoke;
using SkyApm;
using SkyApm.Common;
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

            StringBuilder sbLog = new StringBuilder();
            if (eventData.Data.RemoteInvokeMessage?.Parameters != null)
            {
                sbLog.AppendLine($"Parameters =>");
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

            sbLog.AppendLine($"Token =>");
            sbLog.AppendLine($"{eventData.Data.RemoteInvokeMessage?.Token}");
            if (eventData.Data.RemoteInvokeMessage?.Payload?.Items != null)
            {
                sbLog.AppendLine($"PayLoad =>");
                foreach (var item in eventData.Data.RemoteInvokeMessage.Payload.Items)
                {
                    sbLog.AppendLine($"{item.Key}: {item.Value}");
                }
            }
            else
            {
                sbLog.AppendLine($"PayLoad is null");
            }

            context.Span.AddLog(LogEvent.Event("Invoke start"), LogEvent.Message(sbLog.ToString()));
        }

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeAfter)]
        public void AfterServiceInvoke([Object] ServiceInvokeAfterEventData eventData)
        {
            var context = _entrySegmentContextAccessor.Context;
            if (context == null) return;
            context.Span.AddTag("HasError", eventData.ResultData.HasError);

            var logEvent = $"Invoke finished";
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
