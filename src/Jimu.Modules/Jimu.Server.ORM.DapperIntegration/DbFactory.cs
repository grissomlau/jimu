using Jimu.Database;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Text;

namespace Jimu.Server.ORM.DapperIntegration
{
    public class DbFactory : IDbFactory
    {
        readonly DapperOptions _options;
        public DbFactory(DapperOptions options)
        {
            this._options = options;
        }
        public DbConnection Create()
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
        }
    }
}
