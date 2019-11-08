using Jimu.APM;
using System;

namespace Jimu.Server.APM.EventData.LocalMethod
{
    public class LocalMethodInvokeErrorEventData : LocalMethodInvokeEventData, IErrorEventData
    {
        public LocalMethodInvokeErrorEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public Exception Ex { get; set; }
    }
}
