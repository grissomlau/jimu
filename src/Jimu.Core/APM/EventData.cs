using System;

namespace Jimu.APM
{
    /// <summary>
    /// for diagnostics
    /// </summary>
    public class EventData
    {
        public Guid OperationId { get; set; }
        public string Operation { get; set; }

        public EventData(Guid operationId, string operation)
        {
            this.OperationId = operationId;
            this.Operation = operation;
        }
    }
}
