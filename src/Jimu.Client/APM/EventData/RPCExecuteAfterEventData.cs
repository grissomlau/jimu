using System;

namespace Jimu.Client.APM.EventData
{
    public class RPCExecuteAfterEventData : RPCExecuteEventData
    {
        public RPCExecuteAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public JimuRemoteCallResultData ResultData { get; set; }
    }
}
