using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class LocalMethodInvokeBeforeEventData : LocalMethodInvokeEventData
    {
        public LocalMethodInvokeBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
