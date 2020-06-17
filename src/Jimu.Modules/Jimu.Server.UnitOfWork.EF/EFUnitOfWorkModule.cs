using Autofac;
using Jimu.Module;
using Jimu.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Jimu.Server.UnitOfWork.EF
{
    public class EFUnitOfWorkModule : ServerModuleBase
    {
        readonly EFOptions _options;
        readonly MultipleEFOptions _mulOptions;
        IContainer _container = null;
        public EFUnitOfWorkModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = JimuAppSettings.GetSection(typeof(EFOptions).Name).Get<EFOptions>();
            _mulOptions = JimuAppSettings.GetSection(typeof(MultipleEFOptions).Name).Get<MultipleEFOptions>();
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            Action<LogLevel, string> logAction = null;
            logAction = (level, log) =>
            {
                if (_container != null && _container.IsRegistered<Jimu.Logger.ILogger>())
                {
                    var loggerFactory = _container.Resolve<Jimu.Logger.ILoggerFactory>();
                    var logger = loggerFactory.Create(this.GetType());
                    logger.Info($"【EF】 - LogLevel: {level} - {log}");
                }
            };


            if (_options != null && _options.Enable)
            {
                // register dbfactory
                DbFactory dbFactory = new DbFactory(_options, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return dbFactory;
                }).As<IDbFactory<DbContext>>().InstancePerLifetimeScope();


                EFUnitOfWork unitOfWork = new EFUnitOfWork(_options, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return unitOfWork;
                }).As<UnitOfWorkBase<DbContext>>().As<IUnitOfWork>().InstancePerLifetimeScope();
            }
            else if (_mulOptions != null && _mulOptions.Enable)
            {
                // register dbfactory
                DbFactory dbFactory = new DbFactory(_mulOptions, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return dbFactory;
                }).As<IDbFactory<DbContext>>().InstancePerLifetimeScope();

                EFUnitOfWork unitOfWork = new EFUnitOfWork(_mulOptions, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return unitOfWork;
                }).As<UnitOfWorkBase<DbContext>>().As<IUnitOfWork>().InstancePerLifetimeScope();
            }

            base.DoServiceRegister(serviceContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            _container = container;
            base.DoServiceInit(container);
        }

        public override void DoServiceInit(IContainer container)
        {

            var loggerFactory = container.Resolve<Jimu.Logger.ILoggerFactory>();
            var logger = loggerFactory.Create(this.GetType());
            if (_options != null && _options.Enable)
            {
                logger.Info("EFUnitOfWork Register successfully.");
            }
            else if (_mulOptions != null && _mulOptions.Enable)
            {
                logger.Info("EFUnitOfWork Register successfully.");
            }


            base.DoServiceInit(container);
        }
    }
}
