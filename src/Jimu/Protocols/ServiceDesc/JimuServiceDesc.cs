using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Jimu
{
    public class JimuServiceDesc
    {
        public JimuServiceDesc()
        {
            Metadatas = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        ///     the id for the service(service also as a method)
        /// </summary>
        public string Id
        {
            get => GetMetadata<string>("Id");
            set => Metadatas["Id"] = value;
        }
        /// <summary>
        ///     service 
        /// </summary>
        public string Service
        {
            get => GetMetadata<string>("Service");
            set => Metadatas["Service"] = value;
        }
        /// <summary>
        ///     service comment
        /// </summary>
        public string ServiceComment
        {
            get => GetMetadata<string>("ServiceComment");
            set => Metadatas["ServiceComment"] = value;
        }
        /// <summary>
        ///     service route path
        /// </summary>
        public string RoutePath
        {
            get => GetMetadata<string>("RoutePath");
            set => Metadatas["RoutePath"] = value;
        }

        public bool WaitExecution
        {
            get => GetMetadata<bool>("WaitExecution");
            set => Metadatas["WaitExecution"] = value;
        }

        public bool AllowAnonymous
        {
            get => GetMetadata<bool>("AllowAnonymous");
            set => Metadatas["AllowAnonymous"] = value;
        }

        public string CreatedDate
        {
            get => GetMetadata<string>("CreatedDate");
            set => Metadatas["CreatedDate"] = value;
        }

        public string CreatedBy
        {
            get => GetMetadata<string>("CreatedBy");
            set => Metadatas["CreatedBy"] = value;
        }

        public string Comment
        {
            get => GetMetadata<string>("Comment");
            set => Metadatas["Comment"] = value;
        }

        public string Roles
        {
            get => GetMetadata<string>("Roles");
            set => Metadatas["Roles"] = value;
        }

        public string ReturnDesc
        {
            get => GetMetadata<string>("ReturnDesc");
            set => Metadatas["ReturnDesc"] = value;
        }

        public string HttpMethod
        {
            get => GetMetadata<string>("HttpMethod");
            set => Metadatas["HttpMethod"] = value;
        }
        public string Parameters
        {
            get => GetMetadata<string>("Parameters");
            set => Metadatas["Parameters"] = value;
        }

        public string Rest
        {
            get => GetMetadata<string>("Rest");
            set => Metadatas["Rest"] = value;
        }
        /// <summary>
        ///     other useful data
        /// </summary>
        public IDictionary<string, object> Metadatas { get; set; }

        public T GetMetadata<T>(string name, T def = default(T))
        {
            if (!Metadatas.ContainsKey(name))
                return def;
            return (T)Metadatas[name];
        }
    }
}