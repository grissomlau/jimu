using System;

namespace Jimu.DDD
{
    public class DomainEvent<TKey> : IDomainEvent<TKey>
    {
        public DomainEvent(TKey aggregateRootKey)
        {
            AggregateRootKey = aggregateRootKey;
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
        public virtual TKey AggregateRootKey { get; set; }
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
