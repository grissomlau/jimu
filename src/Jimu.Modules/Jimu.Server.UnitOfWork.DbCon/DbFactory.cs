using Jimu.UnitOfWork;
using System.Data.Common;
using System.Linq;

namespace Jimu.Server.UnitOfWork.DbCon
{
    public class DbFactory : IDbFactory<DbConnection>
    {
        DbConOptions _options;
        public DbFactory(DbConOptions options)
        {
            _options = options;
        }

        MultipleDbConOptions _multipleOptions;
        public DbFactory(MultipleDbConOptions multipleOptions)
        {
            _multipleOptions = multipleOptions;
            _options = _multipleOptions.FirstOrDefault(x => x.IsDefaultOption);
        }
        public DbConnection Create(out DbConOptions options, string optionName = null)
        {
            options = null;
            if (string.IsNullOrEmpty(optionName) || optionName == _options?.OptionName)
            {
                options = _options;
            }
            else if (_multipleOptions != null && _multipleOptions.Any(x => x.OptionName == optionName))
            {
                options = _multipleOptions.FirstOrDefault(x => x.OptionName == optionName);
            }
            if (options == null)
            {
                throw new DbConnectionUnitOfWorkException("Options is null");
            }
            if (string.IsNullOrEmpty(options.DbProviderName))
            {
                throw new DbConnectionUnitOfWorkException("DbProviderName must be specify an value");
            }
            var factory = DbProviderFactories.GetFactory(options.DbProviderName);
            if (null == factory)
            {
                throw new DbConnectionUnitOfWorkException($"DbProvider: {options.DbProviderName} not register in DbProviderFactories");
            }
            var cnn = factory.CreateConnection();
            cnn.ConnectionString = options.ConnectionString;
            return cnn;
        }
        public DbConnection Create(string optionName = null)
        {
            return Create(out DbConOptions options, optionName);
        }
    }
}
