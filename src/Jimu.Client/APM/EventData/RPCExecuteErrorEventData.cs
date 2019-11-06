using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.APM
{
    public class RPCExecuteErrorEventData : RPCExecuteEventData, IErrorEventData
    {
        public RPCExecuteErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
