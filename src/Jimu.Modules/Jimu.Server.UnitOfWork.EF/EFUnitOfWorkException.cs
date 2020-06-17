using System;

namespace Jimu.Server.UnitOfWork.EF
{
    public class EFUnitOfWorkException : Exception
    {
        public EFUnitOfWorkException() { }
        public EFUnitOfWorkException(string msg) : base(msg) { }
        public EFUnitOfWorkException(string msg, Exception innerException) : base(msg, innerException)
        {

        }
    }
}
