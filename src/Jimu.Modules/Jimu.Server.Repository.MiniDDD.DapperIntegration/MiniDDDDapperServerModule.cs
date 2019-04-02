using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Microsoft.Extensions.Configuration;
using MiniDDD.UnitOfWork;

namespace Jimu.Server.Repository.MiniDDD.DapperIntegration
{
    public class MiniDDDDapperServerModule : ServerModuleBase
    {
        readonly MiniDDDDapperOptions _options;
        public MiniDDDDapperServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(MiniDDDDapperOptions).Name).Get<MiniDDDDapperOptions>();
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

                serviceContainerBuilder.RegisterType<IUnitOfWork>().WithParameter("options", dbContextOptions).InstancePerLifetimeScope();

            }
            base.DoServiceRegister(serviceContainerBuilder);
        }

    }
}
