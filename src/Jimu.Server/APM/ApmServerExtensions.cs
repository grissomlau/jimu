using Jimu.APM;
using Jimu.Server.APM.EventData.LocalMethod;
using Jimu.Server.APM.EventData.ServiceInvoke;
using Jimu.Server.Transport;
using System;
using System.Runtime.CompilerServices;

namespace Jimu.Server.APM
{
    public static class ApmServerExtensions
    {

        #region ServiceInvoke

        public static Guid WriteServiceInvokeBefore(this IJimuApm @this, ServiceInvokerContext context)
        {
            if (!@this.IsEnabled(ApmServerEventType.ServiceInvokeBefore)) return Guid.Empty;

            var operationId = Guid.NewGuid();
            var operation = $"Invoke: {context.RemoteInvokeMessage.ServiceId}";

            @this.Write(ApmServerEventType.ServiceInvokeBefore, new ServiceInvokeBeforeEventData(operationId, operation)
            {
                Data = context
            });
            return operationId;
        }

        public static void WriteServiceInvokeAfter(this IJimuApm @this, Guid operationId, ServiceInvokerContext context, JimuRemoteCallResultData resultData)
        {
            if (@this.IsEnabled(ApmServerEventType.ServiceInvokeAfter.ToString()))
            {
                var operation = $"Invoke: {context.RemoteInvokeMessage.ServiceId}";
                @this.Write(ApmServerEventType.ServiceInvokeAfter, new ServiceInvokeAfterEventData(operationId, operation)
                {
                    Data = context,
                    ResultData = resultData
                });
            }
        }

        public static void WriteServiceInvokeError(this IJimuApm @this, Guid operationId, ServiceInvokerContext context, Exception ex)
        {
            if (@this.IsEnabled(ApmServerEventType.ServiceInvokeError))
            {
                var operation = $"Invoke: {context.RemoteInvokeMessage.ServiceId}";
                @this.Write(ApmServerEventType.ServiceInvokeError, new ServiceInvokeErrorEventData(operationId, operation)
                {
                    Ex = ex,
                    Data = context
                });
            }
        }
        #endregion


        #region local method
        public static Guid WriteLocalMethodInvokeBefore(this IJimuApm @this, string invokerData, [CallerMemberName] string operation = "")
        {
            if (!@this.IsEnabled(ApmServerEventType.LocalMethodInvokeBefore)) return Guid.Empty;

            var operationId = Guid.NewGuid();

            @this.Write(ApmServerEventType.LocalMethodInvokeBefore, new LocalMethodInvokeBeforeEventData(operationId, operation)
            {
                Data = invokerData
            });
            return operationId;
        }

        public static void WriteLocalMethodInvokeAfter(this IJimuApm @this, Guid operationId, string invokerData, string resultData, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(ApmServerEventType.LocalMethodInvokeAfter))
            {
                @this.Write(ApmServerEventType.LocalMethodInvokeAfter, new LocalMethodInvokeAfterEventData(operationId, operation)
                {
                    Data = invokerData,
                    ResultData = resultData
                });
            }
        }

        public static void WriteLocalMethodInvokeError(this IJimuApm @this, Guid operationId, string invokeData, Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(ApmServerEventType.LocalMethodInvokeError))
            {
                @this.Write(ApmServerEventType.LocalMethodInvokeError, new LocalMethodInvokeErrorEventData(operationId, operation)
                {
                    Ex = ex,
                    Data = invokeData
                });
            }
        }
        #endregion

    }
}
