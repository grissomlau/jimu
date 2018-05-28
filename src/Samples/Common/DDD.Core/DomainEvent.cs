using System;

namespace DDD.Core
{
    public class DomainEvent : IDomainEvent
    {
        public DomainEvent(object aggregateRootKey)
        {
            AggregateRootKey = aggregateRootKey;
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
        public object AggregateRootKey { get; set; }
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
