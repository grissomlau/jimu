using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class LocalMethodInvokeErrorEventData : LocalMethodInvokeEventData, IErrorEventData
    {
        public LocalMethodInvokeErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
