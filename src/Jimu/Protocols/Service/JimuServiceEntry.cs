using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu
{
    public class JimuServiceEntry
    {
        /// <summary>
        ///     invoke service func: 
        ///     IDictionary is parameters,
        ///     JimuPayload is context,
        ///     return object
        /// </summary>
        public Func<IDictionary<string, object>, JimuPayload, Task<object>> Func { get; set; }

        /// <summary>
        ///     description for the service
        /// </summary>
        public JimuServiceDesc Descriptor { get; set; }
    }
}