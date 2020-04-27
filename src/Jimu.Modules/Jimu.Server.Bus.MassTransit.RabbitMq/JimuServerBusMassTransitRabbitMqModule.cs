using Autofac;
using Jimu.Core.Bus;
using Jimu.Logger;
using Jimu.Server.Bus.MassTransit.RabbitMq.Pattern;
using Jimu.Server.ServiceContainer.Implement.Parser;
using Jimu.Server.Transport;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MT = MassTransit;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class JimuServerBusMassTransitRabbitMqModule : ServerGeneralModuleBase
    {
        readonly MassTransitOptions _options;
        readonly PatternProvider _patternProvider;

        IJimuBus _bus = null;
        IBusControl _massTransitBus = null;
        public JimuServerBusMassTransitRabbitMqModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(MassTransitOptions).Name).Get<MassTransitOptions>();
            _patternProvider = new PatternProvider();
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null && _options.Enable)
            {
                foreach (var pattern in _patternProvider.GetPatterns())
                {
                    pattern.Register(serviceContainerBuilder);
                }

                serviceContainerBuilder.Register(x => _bus);
                serviceContainerBuilder.Register<IBus>(x => _massTransitBus);
            }

            base.DoServiceRegister(serviceContainerBuilder);
        }

        public override async void DoServiceInit(IContainer container)
        {
            if (_options != null && _options.Enable)
            {
                var logger = container.Resolve<ILogger>();
                _massTransitBus = MT.Bus.Factory.CreateUsingRabbitMq(sbc =>
               {
                   var host = sbc.Host($"rabbitmq://{_options.HostAddress}", h =>
                   {
                       if (!string.IsNullOrEmpty(_options.UserName))
                       {
                           h.Username(_options.UserName);
                           h.Password(_options.Password);
                       }

                       foreach (var pattern in _patternProvider.GetPatterns())
                       {
                           pattern.MasstransitConfig(sbc, container, logger, _bus, _options);
                       }

                   });

               });

                _bus = new MassTransitRabbitMqBus(_massTransitBus, _options);

                try
                {
                    await _massTransitBus.StartAsync(); // This is important!

                    logger.Info($"MassTransitRabbitMq start successfully: rabbitmq://{_options.HostAddress}, username: {_options.UserName}, password: {_options.Password}");
                }
                catch (Exception ex)
                {
                    logger.Error($"MassTransitRabbitMq start failed: rabbitmq://{_options.HostAddress}, username: {_options.UserName}, password: {_options.Password}", ex);
                }

            }
            base.DoServiceInit(container);
        }

        public override void DoDispose(IContainer container)
        {
            if (_massTransitBus != null)
            {
                try
                {
                    _massTransitBus.Stop();
                }
                catch (Exception ex)
                {
                }
            }

            base.DoDispose(container);
        }





    }
}
