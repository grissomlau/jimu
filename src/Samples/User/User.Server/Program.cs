using Jimu.Server;
using Jimu.Server.APM;
using Jimu.Server.APM.EventData.ServiceInvoke;
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

            ApplicationServer.Run();
            CreateHostBuilder(args).Build().Run();
            //Console.ReadLine();

            // if run in docker, uncomment bellow code 
            //var host = new HostBuilder();
            //host.RunConsoleAsync().GetAwaiter().GetResult();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          new HostBuilder()
              .ConfigureServices(services => { services.AddSingleton<ITracingDiagnosticProcessor, Processor>(); })
              .AddSkyAPM();
    }

    public class Processor : ITracingDiagnosticProcessor
    {
        public string ListenerName => ApmServerEventType.ListenerName;


        private readonly ITracingContext _tracingContext;
        private readonly IEntrySegmentContextAccessor _entrySegmentContextAccessor;

        public Processor(ITracingContext tracingContext
            , IEntrySegmentContextAccessor exitSegmentContextAccessor
            )
        {
            _tracingContext = tracingContext;
            _entrySegmentContextAccessor = exitSegmentContextAccessor;
        }

        [DiagnosticName(ApmServerEventType.ServiceInvokeBefore)]
        public void BeforeServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            var context = _tracingContext.CreateEntrySegmentContext(eventData.Operation, default);
            context.Span.AddTag("name", "fuck");
        }

        [DiagnosticName(ApmServerEventType.ServiceInvokeAfter)]
        public void AfterServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            _tracingContext.Release(_entrySegmentContextAccessor.Context);
        }

    }

}
