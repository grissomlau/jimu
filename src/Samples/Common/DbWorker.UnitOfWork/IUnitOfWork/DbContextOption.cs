namespace DbWorker.IUnitOfWork
{
    public class DbContextOption
    {
        public string ConnectionString { get; set; }
        public string ModelAssemblyName { get; set; }
        public DbType DbType { get; set; }
    }
    public enum DbType
    {
        MySQL,
        SQLServer,
        Oracle,
        PostgreSQL,
        SQLite,
        MongoDb,
        Redis,
        Memcached
    }
}
