using System;

namespace Jimu.DDD
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }
}
