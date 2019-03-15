using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.ServiceContainer.Implement
{
    public class ServiceOptions
    {
        public string[] AssemblyNames { get; set; }
        public ServiceOptions(string[] assemblyNames)
        {
            this.AssemblyNames = assemblyNames;
        }
        public ServiceOptions() { }
    }
}
