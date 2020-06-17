using System.Collections.Generic;

namespace Jimu.Server.UnitOfWork.SqlSugar
{
    public class MultipleSqlSugarOptions : List<SqlSugarOptions>
    {
        public bool Enable { get; set; } = true;
    }
}
