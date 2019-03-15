using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.Proxy
{
    public class ServiceProxyOptions
    {
        public string[] AssemblyNames { get; set; }
        public ServiceProxyOptions(string[] assemblyNames)
        {
            this.AssemblyNames = assemblyNames;
        }

        public ServiceProxyOptions() { }
    }
}
