using System;
using DbWorker.IUnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace DbWorker.UnitOfWork.EF
{
    public class UnitOfWork : IUnitOfWork.IUnitOfWork
    {
        private readonly SqlClient<DbContext> _sqlClient;
        public UnitOfWork(DefaultDbContext dbContext)
        {
            _sqlClient = new SqlClient<DbContext>(dbContext);
        }
        public void BeginTran()
        {
            _sqlClient.IsBeginTran = true;
        }

        public void CommitTran()
        {
            _sqlClient.IsBeginTran = false;
            _sqlClient.Client.SaveChanges();
        }

        public SqlClient<T> GetSqlClient<T>() where T : class
        {
            if (!typeof(T).IsAssignableFrom(typeof(DefaultDbContext)))
            {
                throw new Exception($"cannot convert {typeof(T)} to {typeof(SqlClient<DefaultDbContext>)}");
            }
            return _sqlClient as SqlClient<T>;
        }

        public void RollbackTran()
        {
            _sqlClient.IsBeginTran = false;
            //_sqlClient.Client.Database.BeginTransaction();
        }
    }
}
