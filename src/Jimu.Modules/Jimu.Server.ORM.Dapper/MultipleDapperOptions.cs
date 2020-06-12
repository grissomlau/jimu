using System.Collections.Generic;

namespace Jimu.Server.ORM.Dapper
{
    public class MultipleDapperOptions : List<DapperOptions>
    {
        public bool Enable { get; set; } = true;
    }
}
