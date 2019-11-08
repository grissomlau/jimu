using System;

namespace Jimu.Server.APM.EventData.LocalMethod
{
    public class LocalMethodInvokeAfterEventData : LocalMethodInvokeEventData
    {
        public LocalMethodInvokeAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public string ResultData { get; set; }
    }
}
