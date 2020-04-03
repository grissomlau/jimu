using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.ORM.Dapper
{
    public class MultipleDapperOptions : List<DapperOptions>
    {
        public bool Enable { get; set; } = true;
    }
}
