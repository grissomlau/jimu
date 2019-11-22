using Jimu.Server;
using Jimu.Server.Diagnostic;
using Jimu.Server.Diagnostic.EventData.ServiceInvoke;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkyApm;
using SkyApm.Agent.GeneralHost;
using SkyApm.Diagnostics;
using SkyApm.Tracing;
using System;
using System.Collections.Generic;

namespace User.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("User Server starting ...");
            ApplicationHostServer.Instance.Run();
        }
    }
}
