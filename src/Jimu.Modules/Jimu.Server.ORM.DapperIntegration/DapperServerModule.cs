using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;
using Autofac;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace Jimu.Server.ORM.DapperIntegration
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
            if (_options != null)
            {
                serviceContainerBuilder.Register((context) =>
                {
                    DbConnection cnn = null;
                    switch (_options.DbType)
                    {
                        case DbType.MySQL:
                            cnn = new MySqlConnection(_options.ConnectionString);
                            break;
                        case DbType.SQLServer:
                            cnn = new SqlConnection(_options.ConnectionString);
                            break;
                        case DbType.Oracle:
                            cnn = new OracleConnection(_options.ConnectionString);
                            break;
                        case DbType.PostgreSQL:
                            cnn = new NpgsqlConnection(_options.ConnectionString);
                            break;
                        case DbType.SQLite:
                            cnn = new SQLiteConnection(_options.ConnectionString);
                            break;
                        default:
                            throw new Exception("please specify DbType!");
                    }
                    return cnn;
                }).As<DbConnection>().InstancePerLifetimeScope();

            }
            base.DoServiceRegister(serviceContainerBuilder);
        }
    }
}
