using System.Data;
using System.Data.Common;
using Jimu.UnitOfWork;

namespace Jimu.Server.UnitOfWork.DbCon
{
    public class DbConnectionUnitOfWork : UnitOfWorkBase<DbConnection>
    {
        DbConnection _worker;
        DbTransaction _dbTransaction;
        bool _hasDisposed;
        DbFactory _dbFactory;


        DbConOptions _options;
        public DbConnectionUnitOfWork(DbConOptions options)
        {
            _options = options;
            _dbFactory = new DbFactory(_options);
        }

        MultipleDbConOptions _multipleOptions;
        public DbConnectionUnitOfWork(MultipleDbConOptions multipleOptions)
        {
            _multipleOptions = multipleOptions;
            _dbFactory = new DbFactory(_multipleOptions);
        }
        public override void BeginTransaction(string optionName = null, IsolationLevel? isolationLevel = null)
        {
            var worker = GetUowWorker(optionName);
            if (!_options.IsSupportTransaction)
                return;
            if (worker.State != ConnectionState.Open)
            {
                worker.Open();
            }

            if (isolationLevel.HasValue)
                _dbTransaction = worker.BeginTransaction(isolationLevel.Value);
            else
                _dbTransaction = worker.BeginTransaction();

        }

        public override void Commit()
        {
            if (!_options.IsSupportTransaction)
                return;
            _dbTransaction.Commit();
            _dbTransaction = null;
        }

        public override void Rollback()
        {
            if (!_options.IsSupportTransaction)
                return;
            _dbTransaction.Rollback();
            _dbTransaction = null;
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        public override DbConnection GetUowWorker(string optionName = null)
        {
            var isCreateNewOne = _worker == null
                || (optionName != _options?.OptionName && _dbTransaction == null);//after commit, the currentTransaction turn to null

            if (isCreateNewOne)
            {
                _worker = _dbFactory.Create(out _options, optionName);
                return _worker;
            }

            if (optionName != _options?.OptionName)
            {
                try
                {
                    _dbTransaction.Rollback();
                    _worker?.Dispose();
                }
                catch { }
                throw new DbConnectionUnitOfWorkException($"existing UowWorker {_options?.OptionName} is running, cannot create an new one {optionName}");
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

        ~DbConnectionUnitOfWork()
        {
            Dispose(false);
        }
    }
}
