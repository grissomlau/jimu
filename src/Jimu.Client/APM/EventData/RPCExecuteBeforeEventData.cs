using System;

namespace Jimu.Client.APM.EventData
{
    public class RPCExecuteBeforeEventData : RPCExecuteEventData
    {
        public RPCExecuteBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
