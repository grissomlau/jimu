using System.Collections.Generic;

namespace Jimu
{
    /// <summary>
    ///     like json-RPC to specify info to invoke remote server method(service)
    /// </summary>
    public class JimuRemoteCallData
    {
        public string ServiceId { get; set; }

        public JimuPayload Payload { get; set; }

        public string Token { get; set; }
        public JimuServiceDesc Descriptor { get; set; }

        public IDictionary<string, object> Parameters { get; set; }
    }

}