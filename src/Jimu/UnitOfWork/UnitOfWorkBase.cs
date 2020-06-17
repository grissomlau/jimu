using System.Data;

namespace Jimu.UnitOfWork
{
    public abstract class UnitOfWorkBase<T> : IUnitOfWork where T : class
    {
        public abstract void BeginTransaction(string optionName = null, IsolationLevel? isolationLevel = null);

        public abstract void Commit();

        public abstract void Dispose();

        public abstract void Rollback();
        /// <summary>
        /// worker for unitOfWork
        /// </summary>
        /// <returns></returns>
        public abstract T GetUowWorker(string optionName = null);
    }
}
