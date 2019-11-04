using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.Diagnostics
{
    public class RPCExecuteBeforeEventData : RPCExecuteEventData
    {
        public RPCExecuteBeforeEventData(Guid operationId, string operation) : base(operationId, operation)
        {
        }
    }
}
