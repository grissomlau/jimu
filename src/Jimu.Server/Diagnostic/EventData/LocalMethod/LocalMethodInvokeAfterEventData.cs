using System;

namespace Jimu.Server.Diagnostic.EventData.LocalMethod
{
    public class LocalMethodInvokeAfterEventData : LocalMethodInvokeEventData
    {
        public LocalMethodInvokeAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public string ResultData { get; set; }
    }
}
