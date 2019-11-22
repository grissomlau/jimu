using Jimu.Client.RemoteCaller;
using System;

namespace Jimu.Client.Diagnostic.EventData
{
    public class RPCExecuteEventData : Jimu.Diagnostic.EventData
    {
        public RPCExecuteEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
        public RemoteCallerContext Data { get; set; }
    }
}
