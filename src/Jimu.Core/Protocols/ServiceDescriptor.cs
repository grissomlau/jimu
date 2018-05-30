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

        public void CreatedDate(string value)
        {
            Metadatas["CreatedDate"] = value;
        }

        public string CreatedDate()
        {
            return GetMetadata<string>("CreatedDate");
        }

        public void CreatedBy(string value)
        {
            Metadatas["CreatedBy"] = value;
        }

        public string CreatedBy()
        {
            return GetMetadata<string>("CreatedBy");
        }

        public void Comment(string value)
        {
            Metadatas["Comment"] = value;
        }

        public string Comment()
        {
            return GetMetadata<string>("Comment");
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