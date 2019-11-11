using System;

namespace Jimu.Server.Diagnostic.EventData.LocalMethod
{
    public class LocalMethodInvokeBeforeEventData : LocalMethodInvokeEventData
    {
        public LocalMethodInvokeBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
