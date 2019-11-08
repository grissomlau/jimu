using Jimu.APM;
using System;

namespace Jimu.Server.APM.EventData.ServiceInvoke
{
    public class ServiceInvokeErrorEventData : ServiceInvokeEventData, IErrorEventData
    {
        public ServiceInvokeErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
