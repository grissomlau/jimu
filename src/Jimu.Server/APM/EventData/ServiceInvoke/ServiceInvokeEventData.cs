using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class ServiceInvokeEventData : EventData
    {
        public ServiceInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public ServiceInvokerContext Data { get; set; }
    }
}
