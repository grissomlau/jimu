using System;

namespace Jimu.Server.ORM.Dapper
{
    public class DapperOptions
    {
        public string ConnectionString { get; set; }
        public DbType DbType { get; set; }
        public bool Enable { get; set; } = true;
        public string OptionName { get; set; }

        public bool IsDefaultOption { get; set; } = true;
    }

    [Flags]
    public enum DbType
    {
        MySQL = 0,
        SQLServer = 1,
        Oracle = 2,
        PostgreSQL = 3,
        SQLite = 4
    }
}
