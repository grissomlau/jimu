using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.APM
{
    public class RPCExecuteAfterEventData : RPCExecuteEventData
    {
        public RPCExecuteAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public JimuRemoteCallResultData ResultData { get; set; }
    }
}
