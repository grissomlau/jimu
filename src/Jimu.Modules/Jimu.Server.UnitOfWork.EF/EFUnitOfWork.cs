using Jimu.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace Jimu.Server.UnitOfWork.EF
{
    public class EFUnitOfWork : UnitOfWorkBase<DbContext>
    {
        DbContext _worker;
        bool _hasDisposed;
        IDbFactory<DbContext> _dbFactory;
        string _currentOptionsName;



        EFOptions _options;
        public EFUnitOfWork(EFOptions options, Action<LogLevel, string> logAction)
        {
            _options = options;
            _dbFactory = new DbFactory(_options, logAction);
        }

        MultipleEFOptions _multipleOptions;
        public EFUnitOfWork(MultipleEFOptions multipleOptions, Action<LogLevel, string> logAction)
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
                worker.Database.BeginTransaction(isolationLevel.Value);
            else
                worker.Database.BeginTransaction();
        }

        public override void Commit()
        {
            if (!_options.IsSupportTransaction)
                return;
            _worker.Database.CommitTransaction();
        }

        public override void Rollback()
        {
            if (!_options.IsSupportTransaction)
                return;
            _worker.Database.RollbackTransaction();
        }

        public override void Dispose()
        {
            Dispose(true);
        }

        public override DbContext GetUowWorker(string optionName = null)
        {
            var isCreateNewOne = _worker == null
                 || (optionName != _currentOptionsName && _worker.Database.CurrentTransaction == null);//after commit, the currentTransaction turn to null

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
                    _worker.Database.CurrentTransaction.Rollback();
                    _worker?.Dispose();
                }
                catch { }
                throw new EFUnitOfWorkException($"existing UowWorker {_options.OptionName} is running, cannot create an new one {_options.OptionName}");
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

        ~EFUnitOfWork()
        {
            Dispose(false);
        }
    }
}
