using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Diagnostics
{
    public class ServiceInvokeEventData : EventData
    {
        public ServiceInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
