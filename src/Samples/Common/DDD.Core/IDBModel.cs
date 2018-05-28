using System;

namespace DDD.Core
{
    public interface IDbModel<out TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
    }
}
