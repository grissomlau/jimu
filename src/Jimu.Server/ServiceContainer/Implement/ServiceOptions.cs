using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.ServiceContainer.Implement
{
    public class ServiceOptions
    {
        public ServiceOptions() { }

        public string Path { get; set; }

        public string WatchFilePattern { get; set; }

        public string LoadFilePattern { get; set; }

        public bool EnableWatch { get; set; }
    }
}
