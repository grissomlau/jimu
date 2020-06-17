using System;

namespace Jimu.Diagnostic
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
