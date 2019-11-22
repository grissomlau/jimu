using Jimu.Diagnostic;
using System;

namespace Jimu.Client.Diagnostic.EventData
{
    public class RPCExecuteErrorEventData : RPCExecuteEventData, IErrorEventData
    {
        public RPCExecuteErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
