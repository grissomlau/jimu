using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Core.Protocols
{
    public class ServiceEntry
    {
        /// <summary>
        ///     invoke service func
        /// </summary>
        public Func<IDictionary<string, object>, Payload, Task<object>> Func { get; set; }

        /// <summary>
        ///     description for the service
        /// </summary>
        public ServiceDescriptor Descriptor { get; set; }
    }
}