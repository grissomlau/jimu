using Autofac;
using Jimu.Module;
using Jimu.UnitOfWork;
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;

namespace Jimu.Server.UnitOfWork.SqlSugar
{
    public class SqlSugarUnitOfWorkModule : ServerModuleBase
    {
        readonly SqlSugarOptions _options;
        readonly MultipleSqlSugarOptions _mulOptions;
        IContainer _container = null;
        public SqlSugarUnitOfWorkModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = JimuAppSettings.GetSection(typeof(SqlSugarOptions).Name).Get<SqlSugarOptions>();
            _mulOptions = JimuAppSettings.GetSection(typeof(MultipleSqlSugarOptions).Name).Get<MultipleSqlSugarOptions>();
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            Action<string> logAction = null;
            logAction = (log) =>
            {
                if (_container != null && _container.IsRegistered<Jimu.Logger.ILogger>())
                {
                    var loggerFactory = _container.Resolve<Jimu.Logger.ILoggerFactory>();
                    var logger = loggerFactory.Create(this.GetType());
                    logger.Info($"【SqlSugar】 - {log}");
                }
            };


            if (_options != null && _options.Enable)
            {
                // register dbfactory
                DbFactory dbFactory = new DbFactory(_options, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return dbFactory;
                }).As<IDbFactory<SqlSugarClient>>().InstancePerLifetimeScope();


                SqlSugarUnitOfWork unitOfWork = new SqlSugarUnitOfWork(_options, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return unitOfWork;
                }).As<UnitOfWorkBase<SqlSugarClient>>().As<IUnitOfWork>().InstancePerLifetimeScope();
            }
            else if (_mulOptions != null && _mulOptions.Enable)
            {
                // register dbfactory
                DbFactory dbFactory = new DbFactory(_mulOptions, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return dbFactory;
                }).As<IDbFactory<SqlSugarClient>>().InstancePerLifetimeScope();

                SqlSugarUnitOfWork unitOfWork = new SqlSugarUnitOfWork(_mulOptions, logAction);
                serviceContainerBuilder.Register((context) =>
                {
                    return unitOfWork;
                }).As<UnitOfWorkBase<SqlSugarClient>>().As<IUnitOfWork>().InstancePerLifetimeScope();
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
                logger.Info("SqlSugarUnitOfWork Register successfully.");
            }
            else if (_mulOptions != null && _mulOptions.Enable)
            {
                logger.Info("SqlSugarUnitOfWork Register successfully.");
            }


            base.DoServiceInit(container);
        }
    }
}
