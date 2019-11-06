using Jimu.Diagnostic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class LocalMethodInvokeEventData : EventData
    {
        public LocalMethodInvokeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public string Data { get; set; }
    }
}
