using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jimu.Server.Transport.Http
{
    public class HttpServer : IServer
    {
        private readonly List<JimuServiceRoute> _serviceRoutes = new List<JimuServiceRoute>();
        private readonly IServiceEntryContainer _serviceEntryContainer;
        private readonly string _ip;
        private readonly int _port;
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;
        private readonly Stack<Func<RequestDel, RequestDel>> _middlewares;
        private readonly Action<IWebHostBuilder> _builderAction;
        private readonly ITypeConvertProvider _typeConvert;
        public HttpServer(string ip, int port, Action<IWebHostBuilder> builderAction, IServiceEntryContainer serviceEntryContainer, ILogger logger, ISerializer serializer, ITypeConvertProvider typeConvert)
        {
            _serviceEntryContainer = serviceEntryContainer;
            _ip = ip;
            _port = port;
            _logger = logger;
            _serializer = serializer;
            _middlewares = new Stack<Func<RequestDel, RequestDel>>();
            _builderAction = builderAction;
            _typeConvert = typeConvert;
        }
        public List<JimuServiceRoute> GetServiceRoutes()
        {
            if (!_serviceRoutes.Any())
            {
                var serviceEntries = _serviceEntryContainer.GetServiceEntry();
                serviceEntries.ForEach(entry =>
                {
                    var serviceRoute = new JimuServiceRoute
                    {
                        Address = new List<JimuAddress> {
                            new HttpAddress(_ip, _port){IsHealth = true}
                            },
                        ServiceDescriptor = entry.Descriptor
                    };
                    _serviceRoutes.Add(serviceRoute);
                });
            }

            return _serviceRoutes;
        }

        public Task StartAsync()
        {
            _logger.Info($"start server: {_ip}:{_port}");

            var builder = new WebHostBuilder()
      .UseKestrel()
      .UseIISIntegration()
      .UseSetting("detailedErrors", "true")
      .UseContentRoot(Directory.GetCurrentDirectory())
      .UseUrls($"http://{_ip}:{_port}")
      .ConfigureServices(services =>
      {
          services.AddSingleton<IStartup>(new Startup(new ConfigurationBuilder().Build(), _middlewares, _serviceEntryContainer, _logger, _serializer, _typeConvert));
      })
       .UseSetting(WebHostDefaults.ApplicationKey, typeof(Startup).GetTypeInfo().Assembly.FullName)
      //.UseStartup<Startup>()
      ;

            //.UseStartup<Startup>()
            //.Build();
            _builderAction?.Invoke(builder);
            var host = builder.Build();
            host.Run();
            var endpoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _logger.Info($"server start successfuly, address is： {endpoint}");
            return Task.CompletedTask;
        }

        public IServer Use(Func<RequestDel, RequestDel> middleware)
        {
            _middlewares.Push(middleware);
            return this;
        }
    }
}
