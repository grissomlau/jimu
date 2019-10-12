using Autofac;
using Microsoft.Extensions.Configuration;

namespace Jimu.Logger
{
    public class Log4netClientModule : ClientModuleBase
    {
        private readonly JimuLog4netOptions _options;
        public Log4netClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = this.JimuAppSettings.GetSection(typeof(JimuLog4netOptions).Name).Get<JimuLog4netOptions>();
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
    }
}
