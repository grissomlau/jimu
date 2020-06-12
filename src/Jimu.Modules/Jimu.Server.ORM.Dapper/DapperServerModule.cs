using Autofac;
using Jimu.Database;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Server.ORM.Dapper
{
    public class DapperServerModule : ServerModuleBase
    {
        readonly DapperOptions _options;
        readonly MultipleDapperOptions _mulOptions;
        public DapperServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = JimuAppSettings.GetSection(typeof(DapperOptions).Name).Get<DapperOptions>();
            _mulOptions = JimuAppSettings.GetSection(typeof(MultipleDapperOptions).Name).Get<MultipleDapperOptions>();
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
                }).As<IDbFactory>().SingleInstance();

            }
            else if (_mulOptions != null && _mulOptions.Enable)
            {
                // register dbfactory
                DbFactory dbFactory = new DbFactory(_mulOptions);
                serviceContainerBuilder.Register((context) =>
                {
                    return dbFactory;
                }).As<IDbFactory>().SingleInstance();


            }
            base.DoServiceRegister(serviceContainerBuilder);
        }
    }
}
