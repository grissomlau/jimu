using Jimu.Server.Transport;
using System;

namespace Jimu.Server.Diagnostic.EventData.ServiceInvoke
{
    public class ServiceInvokeEventData : Jimu.Diagnostic.EventData
    {
        public ServiceInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public ServiceInvokerContext Data { get; set; }
    }
}
