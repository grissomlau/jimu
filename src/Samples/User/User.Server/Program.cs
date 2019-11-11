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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          new HostBuilder()
              .ConfigureServices(services => { services.AddSingleton<ITracingDiagnosticProcessor, Processor>(); })
              .AddSkyAPM();
    }

    public class Processor : ITracingDiagnosticProcessor
    {
        public string ListenerName => DiagnosticServerEventType.ListenerName;


        private readonly ITracingContext _tracingContext;
        private readonly IEntrySegmentContextAccessor _entrySegmentContextAccessor;

        public Processor(ITracingContext tracingContext
            , IEntrySegmentContextAccessor exitSegmentContextAccessor
            )
        {
            _tracingContext = tracingContext;
            _entrySegmentContextAccessor = exitSegmentContextAccessor;
        }

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeBefore)]
        public void BeforeServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            var context = _tracingContext.CreateEntrySegmentContext(eventData.Operation, default);
            context.Span.AddTag("name", "fuck");
        }

        [DiagnosticName(DiagnosticServerEventType.ServiceInvokeAfter)]
        public void AfterServiceInvoke([Object] ServiceInvokeBeforeEventData eventData)
        {
            _tracingContext.Release(_entrySegmentContextAccessor.Context);
        }

    }

}
