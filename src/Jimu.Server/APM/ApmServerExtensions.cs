using Jimu.APM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jimu.Server.APM
{
    public static class ApmServerExtensions
    {

        #region ServiceInvoke

        public static Guid WriteServiceInvokeBefore(this IJimuApm @this, ServiceInvokerContext context)
        {
            if (!@this.IsEnabled(ApmServerType.ServiceInvokeBefore)) return Guid.Empty;

            var operationId = Guid.NewGuid();
            var operation = $"Invoke: {context.RemoteInvokeMessage.ServiceId}";

            @this.Write(ApmServerType.ServiceInvokeBefore, new ServiceInvokeBeforeEventData(operationId, operation)
            {
                Data = context
            });
            return operationId;
        }

        public static void WriteServiceInvokeAfter(this IJimuApm @this, Guid operationId, ServiceInvokerContext context, JimuRemoteCallResultData resultData)
        {
            if (@this.IsEnabled(ApmServerType.ServiceInvokeAfter.ToString()))
            {
                var operation = $"Invoke: {context.RemoteInvokeMessage.ServiceId}";
                @this.Write(ApmServerType.ServiceInvokeAfter, new ServiceInvokeAfterEventData(operationId, operation)
                {
                    Data = context,
                    ResultData = resultData
                });
            }
        }

        public static void WriteServiceInvokeError(this IJimuApm @this, Guid operationId, ServiceInvokerContext context, Exception ex)
        {
            if (@this.IsEnabled(ApmServerType.ServiceInvokeError))
            {
                var operation = $"Invoke: {context.RemoteInvokeMessage.ServiceId}";
                @this.Write(ApmServerType.ServiceInvokeError, new ServiceInvokeErrorEventData(operationId, operation)
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
            if (!@this.IsEnabled(ApmServerType.LocalMethodInvokeBefore)) return Guid.Empty;

            var operationId = Guid.NewGuid();

            @this.Write(ApmServerType.LocalMethodInvokeBefore, new LocalMethodInvokeBeforeEventData(operationId, operation)
            {
                Data = invokerData
            });
            return operationId;
        }

        public static void WriteLocalMethodInvokeAfter(this IJimuApm @this, Guid operationId, string invokerData, string resultData, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(ApmServerType.LocalMethodInvokeAfter))
            {
                @this.Write(ApmServerType.LocalMethodInvokeAfter, new LocalMethodInvokeAfterEventData(operationId, operation)
                {
                    Data = invokerData,
                    ResultData = resultData
                });
            }
        }

        public static void WriteLocalMethodInvokeError(this IJimuApm @this, Guid operationId, string invokeData, Exception ex, [CallerMemberName] string operation = "")
        {
            if (@this.IsEnabled(ApmServerType.LocalMethodInvokeError))
            {
                @this.Write(ApmServerType.LocalMethodInvokeError, new LocalMethodInvokeErrorEventData(operationId, operation)
                {
                    Ex = ex,
                    Data = invokeData
                });
            }
        }
        #endregion

    }
}
