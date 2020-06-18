using System;

namespace Jimu.DDD
{
    public interface IDomainEvent
    {
        string Id { get; }
        DateTime Timestamp { get; }
    }
}
