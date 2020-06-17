using Autofac;
using Jimu.Logger;
using Jimu.Module;
using Jimu.UnitOfWork;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace Jimu.Server.UnitOfWork.DbCon
{
    public class DbConnectionUnitOfWorkModule : ServerModuleBase
    {
        readonly DbConOptions _options;
        readonly MultipleDbConOptions _mulOptions;
        public DbConnectionUnitOfWorkModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = JimuAppSettings.GetSection(typeof(DbConOptions).Name).Get<DbConOptions>();
            _mulOptions = JimuAppSettings.GetSection(typeof(MultipleDbConOptions).Name).Get<MultipleDbConOptions>();
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null && _options.Enable)
            {
                // register dbfactory
                DbFactory dbFactory = new DbFactory(_options);
                serviceContainerBuilder.Register((context) =>
                {
                    return dbFactory;
                }).As<IDbFactory<DbConnection>>().InstancePerLifetimeScope();


                DbConnectionUnitOfWork unitOfWork = new DbConnectionUnitOfWork(_options);
                serviceContainerBuilder.Register((context) =>
                {
                    return unitOfWork;
                }).As<UnitOfWorkBase<DbConnection>>().As<IUnitOfWork>().InstancePerLifetimeScope();
            }
            else if (_mulOptions != null && _mulOptions.Enable)
            {
                // register dbfactory
                DbFactory dbFactory = new DbFactory(_mulOptions);
                serviceContainerBuilder.Register((context) =>
                {
                    return dbFactory;
                }).As<IDbFactory<DbConnection>>().InstancePerLifetimeScope();

                DbConnectionUnitOfWork unitOfWork = new DbConnectionUnitOfWork(_mulOptions);
                serviceContainerBuilder.Register((context) =>
                {
                    return unitOfWork;
                }).As<UnitOfWorkBase<DbConnection>>().As<IUnitOfWork>().InstancePerLifetimeScope();
            }

            base.DoServiceRegister(serviceContainerBuilder);
        }
        public override void DoServiceInit(IContainer container)
        {

            var loggerFactory = container.Resolve<ILoggerFactory>();
            var logger = loggerFactory.Create(this.GetType());
            if (_options != null && _options.Enable)
            {
                logger.Info("DbConUnitOfWork Register successfully.");
            }
            else if (_mulOptions != null && _mulOptions.Enable)
            {
                logger.Info("DbConUnitOfWork Register successfully.");
            }


            base.DoServiceInit(container);
        }

    }
}
