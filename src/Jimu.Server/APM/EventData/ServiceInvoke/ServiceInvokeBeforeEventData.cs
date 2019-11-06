using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.APM
{
    public class ServiceInvokeBeforeEventData : ServiceInvokeEventData
    {
        public ServiceInvokeBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
