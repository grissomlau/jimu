using System;

namespace Jimu.Server.APM.EventData.LocalMethod
{
    public class LocalMethodInvokeEventData : Jimu.APM.EventData
    {
        public LocalMethodInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public string Data { get; set; }
    }
}
