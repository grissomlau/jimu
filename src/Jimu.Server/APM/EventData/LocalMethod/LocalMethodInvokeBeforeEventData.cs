using System;

namespace Jimu.Server.APM.EventData.LocalMethod
{
    public class LocalMethodInvokeBeforeEventData : LocalMethodInvokeEventData
    {
        public LocalMethodInvokeBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
