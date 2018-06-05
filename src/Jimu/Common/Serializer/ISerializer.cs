using System;

namespace Jimu
{
    public interface ISerializer
    {
        T Serialize<T>(object instance);

        TResult Deserialize<T, TResult>(T data) where TResult : class;
        object Deserialize<T>(T data, Type type);
    }
}