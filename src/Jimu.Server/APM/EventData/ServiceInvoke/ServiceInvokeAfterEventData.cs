using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class ServiceInvokeAfterEventData : ServiceInvokeEventData
    {
        public ServiceInvokeAfterEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }

        public JimuRemoteCallResultData ResultData { get; set; }
    }
}
