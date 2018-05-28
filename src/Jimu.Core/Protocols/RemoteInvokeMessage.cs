using System.Collections.Generic;

namespace Jimu.Core.Protocols
{
    /// <summary>
    ///     like json-RPC to specify info to invoke remote server method(service)
    /// </summary>
    public class RemoteInvokeMessage
    {
        public string ServiceId { get; set; }

        public Payload Payload { get; set; }

        public string Token { get; set; }
        public ServiceDescriptor Descriptor { get; set; }

        public IDictionary<string, object> Parameters { get; set; }
    }

    public class Payload
    {
        public IDictionary<string, object> Items { get; set; }
        //public const string PayloadParameterName = "__payload__";
    }
}