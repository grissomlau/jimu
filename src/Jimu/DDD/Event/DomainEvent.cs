using System;

namespace Jimu.DDD
{
    public class DomainEvent : IDomainEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
