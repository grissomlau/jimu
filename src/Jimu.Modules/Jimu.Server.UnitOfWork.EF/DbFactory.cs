using Jimu.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Jimu.Server.UnitOfWork.EF
{
    public class DbFactory : IDbFactory<DbContext>
    {
        EFOptions _options;
        readonly Action<LogLevel, string> _logAction;
        public DbFactory(EFOptions options, Action<LogLevel, string> logAction)
        {
            _options = options;
            _logAction = logAction;
        }

        MultipleEFOptions _multipleOptions;
        public DbFactory(MultipleEFOptions multipleOptions, Action<LogLevel, string> logAction)
        {
            _multipleOptions = multipleOptions;
            _options = _multipleOptions.FirstOrDefault(x => x.IsDefaultOption);
            _logAction = logAction;
        }
        public DbContext Create(string optionName = null)
        {
            EFOptions options = null;
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
                throw new EFUnitOfWorkException("Options is null");
            }
            EFUnitOfWorkDbContext context = new EFUnitOfWorkDbContext(options, _logAction);
            return context;
        }
    }
}
