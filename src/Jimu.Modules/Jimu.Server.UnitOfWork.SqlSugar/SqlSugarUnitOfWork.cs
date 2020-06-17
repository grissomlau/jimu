using System;
using System.Data;
using Jimu.UnitOfWork;
using SqlSugar;

namespace Jimu.Server.UnitOfWork.SqlSugar
{
    public class SqlSugarUnitOfWork : UnitOfWorkBase<SqlSugarClient>
    {
        SqlSugarClient _worker;
        bool _hasDisposed;
        IDbFactory<SqlSugarClient> _dbFactory;
        string _currentOptionsName;



        SqlSugarOptions _options;
        public SqlSugarUnitOfWork(SqlSugarOptions options, Action<string> logAction)
        {
            _options = options;
            _dbFactory = new DbFactory(_options, logAction);
        }

        MultipleSqlSugarOptions _multipleOptions;
        public SqlSugarUnitOfWork(MultipleSqlSugarOptions multipleOptions, Action<string> logAction)
        {
            _multipleOptions = multipleOptions;
            _dbFactory = new DbFactory(_multipleOptions, logAction);
        }
        public override void BeginTransaction(string optionName = null, IsolationLevel? isolationLevel = null)
        {
            var worker = GetUowWorker(optionName);
            if (!_options.IsSupportTransaction)
                return;

            if (isolationLevel.HasValue)
                worker.Ado.BeginTran(isolationLevel.Value);
            else
                worker.Ado.BeginTran();
        }

        public override void Commit()
        {
            if (!_options.IsSupportTransaction)
                return;
            _worker.Ado.CommitTran();
        }

        public override void Rollback()
        {
            if (!_options.IsSupportTransaction)
                return;
            _worker.Ado.RollbackTran();
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        public override SqlSugarClient GetUowWorker(string optionName = null)
        {

            var isCreateNewOne = _worker == null
                 || (optionName != _currentOptionsName && _worker.Ado.Transaction == null);//after commit, the currentTransaction turn to null

            if (isCreateNewOne)
            {
                _worker = _dbFactory.Create(optionName);
                _currentOptionsName = optionName;
                return _worker;
            }

            if (optionName != _currentOptionsName)
            {
                try
                {
                    _worker.Ado.RollbackTran();
                    _worker?.Dispose();
                }
                catch { }
                throw new SqlSugarUnitOfWorkException($"existing UowWorker {_options.OptionName} is running, cannot create an new one {_options.OptionName}");
            }
            return _worker;
        }


        protected void Dispose(bool disposeAll)
        {
            if (_hasDisposed)
            {
                return;
            }
            if (disposeAll)
            {
                // dispose hosting source
            }
            // dispose unhosting source
            if (null != _worker)
            {
                try
                {
                    _worker.Dispose();
                }
                catch
                {

                }
                finally
                {
                    _worker = null;
                }
            }
            _hasDisposed = true;

        }

        ~SqlSugarUnitOfWork()
        {
            Dispose(false);
        }
    }
}
