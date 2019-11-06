using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class LocalMethodInvokeAfterEventData : LocalMethodInvokeEventData
    {
        public LocalMethodInvokeAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public string ResultData { get; set; }
    }
}
