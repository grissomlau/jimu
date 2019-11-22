using Jimu.Diagnostic;
using System;

namespace Jimu.Server.Diagnostic.EventData.ServiceInvoke
{
    public class ServiceInvokeErrorEventData : ServiceInvokeEventData, IErrorEventData
    {
        public ServiceInvokeErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
