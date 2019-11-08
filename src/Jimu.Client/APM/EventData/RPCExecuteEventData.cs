using Jimu.Client.RemoteCaller;
using System;

namespace Jimu.Client.APM.EventData
{
    public class RPCExecuteEventData : Jimu.APM.EventData
    {
        public RPCExecuteEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
        public RemoteCallerContext Data { get; set; }
    }
}
