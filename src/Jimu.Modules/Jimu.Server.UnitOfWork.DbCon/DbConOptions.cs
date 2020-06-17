namespace Jimu.Server.UnitOfWork.DbCon
{
    public class DbConOptions
    {
        public bool Enable { get; set; } = true;

        public string ConnectionString { get; set; }

        public string DbProviderName { get; set; }

        public string OptionName { get; set; }

        public bool IsDefaultOption { get; set; } = true;
        public bool IsSupportTransaction { get; set; } = true;
    }
}
