using System;

namespace Jimu.Server.APM.EventData.ServiceInvoke
{
    public class ServiceInvokeBeforeEventData : ServiceInvokeEventData
    {
        public ServiceInvokeBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
