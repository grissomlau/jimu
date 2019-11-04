using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.Diagnostics
{
    public class RPCExecuteEventData : EventData
    {
        public RPCExecuteEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
        public RemoteCallerContext Context { get; set; }
    }
}
