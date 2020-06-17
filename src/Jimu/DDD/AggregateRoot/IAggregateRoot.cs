using System;
using System.Collections.Generic;

namespace Jimu.DDD
{
    public interface IAggregateRoot<TKey> :
        IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        IEnumerable<IDomainEvent<TKey>> UncommittedEvents { get; }

        void Replay(IEnumerable<IDomainEvent<TKey>> events);
    }
}
