using System;

namespace Jimu.Server.APM.EventData.ServiceInvoke
{
    public class ServiceInvokeAfterEventData : ServiceInvokeEventData
    {
        public ServiceInvokeAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public JimuRemoteCallResultData ResultData { get; set; }
    }
}
