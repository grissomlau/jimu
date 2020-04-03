using Jimu.Database;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;

namespace Jimu.Server.ORM.Dapper
{
    public class DbFactory : IDbFactory
    {
        readonly DapperOptions _options;
        readonly MultipleDapperOptions _mulOptions;
        public DbFactory(DapperOptions options)
        {
            this._options = options;
        }
        public DbFactory(MultipleDapperOptions mulOptions)
        {
            _mulOptions = mulOptions;
            _options = _mulOptions.FirstOrDefault(x => x.IsDefaultOption);
        }

        public DbConnection Create(DapperOptions options)
        {
            DbConnection cnn;
            switch (_options.DbType)
            {
                case DbType.MySQL:
                    cnn = new MySqlConnection(options.ConnectionString);
                    break;
                case DbType.SQLServer:
                    cnn = new SqlConnection(options.ConnectionString);
                    break;
                case DbType.Oracle:
                    cnn = new OracleConnection(options.ConnectionString);
                    break;
                case DbType.PostgreSQL:
                    cnn = new NpgsqlConnection(options.ConnectionString);
                    break;
                case DbType.SQLite:
                    cnn = new SQLiteConnection(options.ConnectionString);
                    break;
                default:
                    throw new Exception("please specify DbType!");
            }
            return cnn;
        }

        public DbConnection Create(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return this.Create(_options);
            }
            if (_mulOptions != null && _mulOptions.Any(x => x.OptionName == name))
            {
                var options = _mulOptions.FirstOrDefault(x => x.OptionName == name);
                return Create(options);
            }
            throw new Exception($"MultipleDapperOptions name:  {name} not found!");
        }
    }
}
