using System;

namespace DDD.Core
{
    public interface IRepository<T, in TKey>
        where T : AggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        T Get(TKey key);

        void Save(T aggreateRoot);
    }
}
