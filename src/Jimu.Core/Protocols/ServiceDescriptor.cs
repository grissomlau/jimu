using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Jimu.Core.Protocols
{
    public class ServiceDescriptor
    {
        public ServiceDescriptor()
        {
            Metadatas = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        ///     the id for the service(service also as a method)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     service route path
        /// </summary>
        public string RoutePath { get; set; }

        /// <summary>
        ///     other useful data
        /// </summary>
        public IDictionary<string, object> Metadatas { get; set; }


        public void WaitExecution(bool value)
        {
            Metadatas["WaitExecution"] = value;
        }

        public bool WaitExecution()
        {
            return GetMetadata("WaitExecution", true);
        }

        public void EnableAuthorization(bool value)
        {
            Metadatas["EnableAuthorization"] = value;
        }

        public bool EnableAuthorization()
        {
            return GetMetadata("EnableAuthorization", false);
        }

        public void Date(string value)
        {
            Metadatas["Date"] = value;
        }

        public string Date()
        {
            return GetMetadata<string>("Date");
        }

        public void Director(string value)
        {
            Metadatas["Director"] = value;
        }

        public string Director()
        {
            return GetMetadata<string>("Director");
        }

        public void Name(string value)
        {
            Metadatas["Name"] = value;
        }

        public string Name()
        {
            return GetMetadata<string>("Name");
        }

        public string Roles()
        {
            return GetMetadata<string>("Roles");
        }

        public void Roles(string value)
        {
            Metadatas["Roles"] = value;
        }

        public string ReturnType()
        {
            return GetMetadata<string>("ReturnType");
        }

        public void ReturnType(string value)
        {
            Metadatas["ReturnType"] = value;
        }

        public T GetMetadata<T>(string name, T def = default(T))
        {
            if (!Metadatas.ContainsKey(name))
                return def;
            return (T) Metadatas[name];
        }
    }
}