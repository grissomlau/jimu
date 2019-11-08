using Autofac;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Logger.Log4net
{
    public class Log4netServerModule : ServerModuleBase
    {
        private readonly JimuLog4netOptions _options;
        public Log4netServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = this.JimuAppSettings.GetSection("JimuLog4netOptions").Get<JimuLog4netOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                componentContainerBuilder.RegisterType<Log4netLogger>().WithParameter("options", _options).As<ILogger>().SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use Log4net logger");
            }
            base.DoInit(container);
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null & _options.UseInService)
            {

                serviceContainerBuilder.RegisterType<Log4netLogger>().WithParameter("options", _options).As<ILogger>().SingleInstance();
            }
            base.DoServiceRegister(serviceContainerBuilder);
        }
    }
}
