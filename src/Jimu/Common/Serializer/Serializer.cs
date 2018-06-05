using System;
using System.Text;
using Newtonsoft.Json;

namespace Jimu
{
    public class Serializer : ISerializer
    {
        static Serializer()
        {
            InitSerializer();
            InitDeserializer();
        }

        public TResult Deserialize<T, TResult>(T content) where TResult : class
        {
            return DeserializeProxy<T>.DeserializeFunc(content, typeof(TResult)) as TResult;
        }

        public object Deserialize<T>(T content, Type type)
        {
            return DeserializeProxy<T>.DeserializeFunc(content, type);
        }

        public T Serialize<T>(object instance)
        {
            return SerializerProxy<T>.SerializeFunc(instance);
        }

        private static void InitSerializer()
        {
            SerializerProxy<string>.SerializeFunc = JsonConvert.SerializeObject;
            SerializerProxy<byte[]>.SerializeFunc = instance => Encoding.UTF8.GetBytes(SerializerProxy<string>.SerializeFunc(instance));
        }

        private static void InitDeserializer()
        {
            DeserializeProxy<object>.DeserializeFunc = (content, type) => DeserializeProxy<string>.DeserializeFunc(content.ToString(), type);
            DeserializeProxy<string>.DeserializeFunc = JsonConvert.DeserializeObject;

            DeserializeProxy<byte[]>.DeserializeFunc = (content, type) => DeserializeProxy<string>.DeserializeFunc(Encoding.UTF8.GetString(content), type);
        }


        private static class SerializerProxy<T>
        {
            public static Func<object, T> SerializeFunc;
        }

        private static class DeserializeProxy<T>
        {
            public static Func<T, Type, object> DeserializeFunc;
        }
    }
}