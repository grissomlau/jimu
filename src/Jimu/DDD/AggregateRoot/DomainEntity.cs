using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.DDD
{
    public abstract class DomainEntity<TAggregate, TKey> : IEntity where TKey : IEquatable<TKey> where TAggregate : IEntity
    {
        public virtual TKey Id { get; set; }

        public virtual TAggregate Aggregate { get; set; }
    }
}
