using System;

namespace Jimu.Server.UnitOfWork.SqlSugar
{
    public class SqlSugarUnitOfWorkException : Exception
    {
        public SqlSugarUnitOfWorkException() { }
        public SqlSugarUnitOfWorkException(string msg) : base(msg) { }
        public SqlSugarUnitOfWorkException(string msg, Exception innerException) : base(msg, innerException)
        {

        }
    }
}
