using System.Collections.Generic;

namespace Jimu.Server.UnitOfWork.DbCon
{
    public class MultipleDbConOptions : List<DbConOptions>
    {
        public bool Enable { get; set; } = true;
    }
}
