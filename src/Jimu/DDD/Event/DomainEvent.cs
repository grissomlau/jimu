using System;

namespace Jimu.DDD
{
    public class DomainEvent : IDomainEvent
    {
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();
        public virtual DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
