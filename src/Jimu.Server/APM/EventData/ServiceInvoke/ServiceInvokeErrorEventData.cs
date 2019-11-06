using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class ServiceInvokeErrorEventData : ServiceInvokeEventData, IErrorEventData
    {
        public ServiceInvokeErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
