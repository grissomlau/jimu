using System;

namespace Jimu.DDD
{
    public interface IRepository<T, in TKey>
        where T : AggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        T Get(TKey key);

        void Save(T aggreateRoot);
    }
}
