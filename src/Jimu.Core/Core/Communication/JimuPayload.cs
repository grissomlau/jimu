using System.Collections.Generic;

namespace Jimu
{
    /// <summary>
    ///     store client meta info when using jwt to authorize the client
    /// </summary>
    public class JimuPayload
    {
        public IDictionary<string, object> Items { get; set; } = new Dictionary<string, object>();
    }
}
