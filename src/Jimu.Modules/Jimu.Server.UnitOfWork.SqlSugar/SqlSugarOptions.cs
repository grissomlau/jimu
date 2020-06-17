using SqlSugar;

namespace Jimu.Server.UnitOfWork.SqlSugar
{
    public class SqlSugarOptions
    {
        public bool Enable { get; set; } = true;

        public string ConnectionString { get; set; }

        public DbType DbType { get; set; }

        public string OptionName { get; set; }

        public bool IsDefaultOption { get; set; } = true;
        public bool IsSupportTransaction { get; set; } = true;
        public bool OpenLogTrace { get; set; }
    }
}
