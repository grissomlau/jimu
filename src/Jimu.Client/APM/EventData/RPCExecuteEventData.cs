using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.APM
{
    public class RPCExecuteEventData : EventData
    {
        public RPCExecuteEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
        public RemoteCallerContext Data { get; set; }
    }
}
