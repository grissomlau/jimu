using System;
using DbWorker.IUnitOfWork;
using Sugar = SqlSugar;

namespace DbWorker.UnitOfWork.SqlSugar
{
    public class UnitOfWork : IUnitOfWork.IUnitOfWork
    {
        //private readonly ConcurrentDictionary<string, SqlClient<SqlSugarClient>> _sqlClients = new ConcurrentDictionary<string, SqlClient<SqlSugarClient>>();
        //private readonly ThreadLocal<string> _threadLocal;
        /// <summary>
        /// diffrent vaue for every thread
        /// </summary>
        //private ThreadLocal<SqlClient<SqlSugarClient>> _tlSqlClient;
        private SqlClient<Sugar.SqlSugarClient> _sqlClient;
        private readonly DbContextOption _options;

        public UnitOfWork(DbContextOption options)
        {
            _options = options;
        }
        public void BeginTran()
        {
            var sqlClient = GetSqlClient();
            if (sqlClient?.Client != null)
            {
                if (!sqlClient.IsBeginTran)
                    sqlClient.Client.Ado.BeginTran();
                sqlClient.IsBeginTran = true;
            }
        }

        public void CommitTran()
        {
            if (_sqlClient?.Client != null && _sqlClient.IsBeginTran)
            {
                _sqlClient.Client.Ado.CommitTran();
            }
        }

        public void Release()
        {
            _sqlClient?.Client?.Dispose();
        }

        private SqlClient<Sugar.SqlSugarClient> GetSqlClient()
        {
            if (_sqlClient == null)
            {
                var client = new Sugar.SqlSugarClient(new Sugar.ConnectionConfig
                {
                    IsAutoCloseConnection = true,
                    ConnectionString = _options.ConnectionString,
                    DbType = (Sugar.DbType)Enum.Parse(typeof(Sugar.DbType), _options.DbType.ToString(), true),
                    InitKeyType = Sugar.InitKeyType.SystemTable

                });
                client.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine($"SQL: {sql}");
                };
                _sqlClient = new SqlClient<Sugar.SqlSugarClient>(client);
            }
            Console.WriteLine("sqlclient id is " + _sqlClient.Id);
            return _sqlClient;
        }

        public void RollbackTran()
        {
            if (_sqlClient?.Client != null && _sqlClient.IsBeginTran)
            {
                _sqlClient.Client.Ado.RollbackTran();
            }
        }

        public SqlClient<T> GetSqlClient<T>() where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(Sugar.SqlSugarClient)))
            {
                throw new InvalidCastException($"cannot convert {typeof(T)} to SqlSugarClient");
            }
            return GetSqlClient() as SqlClient<T>;
        }
    }
}
