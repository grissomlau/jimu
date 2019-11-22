using Jimu.Diagnostic;
using Jimu.Server.Diagnostic.EventData.LocalMethod;
using Jimu.Server.Diagnostic.EventData.ServiceInvoke;
using Jimu.Server.Transport;
using System;
using System.Runtime.CompilerServices;

namespace Jimu.Server.Diagnostic
{
    public static class DiagnosticServerExtensions
    {

        #region ServiceInvoke

        public static Guid WriteServiceInvokeBefore(this IJimuDiagnostic @this, ServiceInvokerContext context)
        {
            if (!@this.IsEnabled(DiagnosticServerEventType.ServiceInvokeBefore)) return Guid.Empty;

            var operationId = Guid.NewGuid();
            var operation = context.RemoteInvokeMessage.ServiceId;

            @this.Write(DiagnosticServerEventType.ServiceInvokeBefore, new ServiceInvokeBeforeEventData(operationId, operation)
            {
                Data = context
            });
            return operationId;
        }

        public static void WriteServiceInvokeAfter(this IJimuDiagnostic @this, Guid operationId, ServiceInvokerContext context, JimuRemoteCallResultData resultData)
        {
            if (@this.IsEnabled(DiagnosticServerEventType.ServiceInvokeAfter.ToString()))
            {
                var operation = context.RemoteInvokeMessage.ServiceId;
                @this.Write(DiagnosticServerEventType.ServiceInvokeAfter, new ServiceInvokeAfterEventData(operationId, operation)
                {
                    Data = context,
                    ResultData = resultData
                });
            }
        }

        public static void WriteServiceInvokeError(this IJimuDiagnostic @this, Guid operationId, ServiceInvokerContext context, Exception ex)
        {
            if (@this.IsEnabled(DiagnosticServerEventType.ServiceInvokeError))
            {
                var operation =context.RemoteInvokeMessage.ServiceId;
                @this.Write(DiagnosticServerEventType.ServiceInvokeError, new ServiceInvokeErrorEventData(operationId, operation)
                {
                    Ex = ex,
                    Data = context
                });
            }
        }
        #endregion


        #region local method
        public static Guid WriteLocalMethodInvokeBefore(this IJimuDiagnostic @this, JimuPayload payload, string invokerData, [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(DiagnosticServerEventType.LocalMethodInvokeBefore)) return Guid.Empty;

            var operationId = Guid.NewGuid();

            @this.Write(DiagnosticServerEventType.LocalMethodInvokeBefore, new LocalMethodInvokeBeforeEventData(operationId,operation)
            {
                Data = invokerData,
                Payload = payload
            });
            return operationId;
        }

        public static void WriteLocalMethodInvokeAfter(this IJimuDiagnostic @this, Guid operationId, string invokerData, string resultData, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(DiagnosticServerEventType.LocalMethodInvokeAfter))
            {
                @this.Write(DiagnosticServerEventType.LocalMethodInvokeAfter, new LocalMethodInvokeAfterEventData(operationId, operation)
                {
                    Data = invokerData,
                    ResultData = resultData
                });
            }
        }

        public static void WriteLocalMethodInvokeError(this IJimuDiagnostic @this, Guid operationId, string invokeData, Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(DiagnosticServerEventType.LocalMethodInvokeError))
            {
                @this.Write(DiagnosticServerEventType.LocalMethodInvokeError, new LocalMethodInvokeErrorEventData(operationId, operation)
                {
                    Ex = ex,
                    Data = invokeData
                });
            }
        }
        #endregion

    }
}
