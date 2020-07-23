using Autofac;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Logger.NLog
{
    public class NLogClientModule : ClientModuleBase
    {
        private readonly JimuNLogOptions _options;
        private readonly NLoggerFactory _nLoggerFactory;
        public NLogClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = this.JimuAppSettings.GetSection(typeof(JimuNLogOptions).Name).Get<JimuNLogOptions>();
            if (_options != null)
            {
                _nLoggerFactory = new NLoggerFactory(_options);
            }
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                //componentContainerBuilder.RegisterType<NLogger>().WithParameter("options", _options).As<ILogger>().SingleInstance();
                //componentContainerBuilder.RegisterType<NLoggerFactory>().WithParameter("options", _options).As<ILoggerFactory>().SingleInstance();
                componentContainerBuilder.RegisterInstance(_nLoggerFactory).As<ILoggerFactory>().SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var loggerFactory = container.Resolve<ILoggerFactory>();
                var logger = loggerFactory.Create(this.GetType());
                logger.Info($"[config]use NLog logger");
            }
            base.DoInit(container);
        }
    }
}
