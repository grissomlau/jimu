using System;

namespace Jimu.Server.Diagnostic.EventData.LocalMethod
{
    public class LocalMethodInvokeEventData : Jimu.Diagnostic.EventData
    {
        public LocalMethodInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public string Data { get; set; }
    }
}
