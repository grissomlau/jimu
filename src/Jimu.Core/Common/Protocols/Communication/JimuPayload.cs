using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    /// <summary>
    ///     store client meta info when using jwt to authorize the client
    /// </summary>
    public class JimuPayload
    {
        public IDictionary<string, object> Items { get; set; }
    }
}
