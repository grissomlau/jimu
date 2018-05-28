using System;

namespace DDD.Core
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        object AggregateRootKey { get; set; }

        DateTime Timestamp { get; }
    }
}
