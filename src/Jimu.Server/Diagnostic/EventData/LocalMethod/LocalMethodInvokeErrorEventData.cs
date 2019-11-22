using Jimu.Diagnostic;
using System;

namespace Jimu.Server.Diagnostic.EventData.LocalMethod
{
    public class LocalMethodInvokeErrorEventData : LocalMethodInvokeEventData, IErrorEventData
    {
        public LocalMethodInvokeErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
