using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiniDDD.UnitOfWork;
using MiniDDD.UnitOfWork.EF;

namespace Jimu.Server.Repository.MiniDDD.EFIntegration
{
    public class MiniDDDEFServerModule : ServerModuleBase
    {
        readonly MiniDDDEFOptions _options;
        IContainer _container = null;
        public MiniDDDEFServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(MiniDDDEFOptions).Name).Get<MiniDDDEFOptions>();
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null)
            {
                DbContextOptions dbContextOptions = new DbContextOptions
                {
                    ConnectionString = _options.ConnectionString,
                    DbType = _options.DbType
                };
                Action<LogLevel, string> logAction = null;
                if (_options.OpenLogTrace)
                {
                    logAction = (level, log) =>
                    {
                        if (_container != null && _container.IsRegistered<Jimu.Logger.ILogger>())
                        {
                            Jimu.Logger.ILogger logger = _container.Resolve<Jimu.Logger.ILogger>();
                            logger.Info($"【EF】 - LogLevel: {level} - {log}");
                        }
                    };
                }

                DefaultDbContext dbContext = new DefaultDbContext(dbContextOptions, _options.TableModelAssemblyName, logAction, _options.LogLevel);
                serviceContainerBuilder.RegisterType<UnitOfWork>()
                    .WithMetadata("context", dbContext)
                    .InstancePerLifetimeScope();
            }
            base.DoServiceRegister(serviceContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            _container = container;
            base.DoServiceInit(container);
        }
    }
}
