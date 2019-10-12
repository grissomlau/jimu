using Autofac;
using Microsoft.Extensions.Configuration;

namespace Jimu.Logger
{
    public class NLogServerModule : ServerModuleBase
    {
        private readonly JimuNLogOptions _options;
        public NLogServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = this.JimuAppSettings.GetSection(typeof(JimuNLogOptions).Name).Get<JimuNLogOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                componentContainerBuilder.RegisterType<NLogger>().WithParameter("options", _options).As<ILogger>().SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use NLog logger");
            }
            base.DoInit(container);
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null && _options.UseInService)
            {

                serviceContainerBuilder.RegisterType<NLogger>().WithParameter("options", _options).As<ILogger>().SingleInstance();
            }
            base.DoServiceRegister(serviceContainerBuilder);
        }
    }
}
