using System;
using System.Data;

namespace Jimu.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //T GetSqlClient<T>() where T : class;
        void BeginTransaction(string optionName = null, IsolationLevel? isolationLevel = null);
        void Commit();
        void Rollback();
    }
}
