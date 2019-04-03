using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Microsoft.Extensions.Configuration;
using MiniDDD;
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

                // register repository
                var repositoryType = typeof(InlineEventHandler);
                var assembies = AppDomain.CurrentDomain.GetAssemblies();
                var repositories = assembies.SelectMany(x => x.GetTypes()).Where(x =>
                {
                    if (x.IsClass && !x.IsAbstract && typeof(InlineEventHandler).IsAssignableFrom(x))
                    {
                        foreach (var face in x.GetInterfaces())
                        {
                            return face.IsGenericType && face.GetGenericTypeDefinition() == typeof(IRepository<,>);
                        }
                    }
                    return false;
                }).ToList();
                repositories.ForEach(x => serviceContainerBuilder.RegisterType(x).AsImplementedInterfaces().InstancePerLifetimeScope());

            }
            base.DoServiceRegister(serviceContainerBuilder);
        }

    }
}
