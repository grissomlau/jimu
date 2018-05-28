using System;

namespace Jimu.Core.Commons.Serializer
{
    public interface ISerializer
    {
        T Serialize<T>(object instance);

        TResult Deserialize<T, TResult>(T data) where TResult : class;
        object Deserialize<T>(T data, Type type);
    }
}