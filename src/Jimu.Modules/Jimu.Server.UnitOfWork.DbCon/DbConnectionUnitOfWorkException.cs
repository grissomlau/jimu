using System;

namespace Jimu.Server.UnitOfWork.DbCon
{
    public class DbConnectionUnitOfWorkException : Exception
    {
        public DbConnectionUnitOfWorkException() { }
        public DbConnectionUnitOfWorkException(string msg) : base(msg) { }
        public DbConnectionUnitOfWorkException(string msg, Exception innerException) : base(msg, innerException)
        {

        }
    }
}
