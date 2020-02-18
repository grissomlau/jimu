using Autofac;
using Jimu.Database;
using Jimu.Module;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Jimu.Server.ORM.Dapper
{
    public class DapperServerModule : ServerModuleBase
    {
        readonly DapperOptions _options;
        public DapperServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = JimuAppSettings.GetSection(typeof(DapperOptions).Name).Get<DapperOptions>();
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
            base.DoServiceRegister(serviceContainerBuilder);
        }
    }
}
