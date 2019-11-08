using Jimu.Server.Transport;
using System;

namespace Jimu.Server.APM.EventData.ServiceInvoke
{
    public class ServiceInvokeEventData : Jimu.APM.EventData
    {
        public ServiceInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public ServiceInvokerContext Data { get; set; }
    }
}
