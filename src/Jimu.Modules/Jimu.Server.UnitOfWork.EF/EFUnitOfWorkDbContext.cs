using Jimu.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jimu.Server.UnitOfWork.EF
{
    public class EFUnitOfWorkDbContext : DbContext
    {
        private LoggerFactory _loggerFactory;
        readonly Action<LogLevel, string> _logAction;
        List<Type> _modelTypes;

        List<Type> ModelTypes
        {
            get
            {
                if (_modelTypes == null)
                {
                    _modelTypes = new List<Type>();
                    if (!string.IsNullOrEmpty(_options.TableModelAssemblyName))
                    {
                        var assembly = Assembly.Load(_options.TableModelAssemblyName);
                        var types = assembly?.GetTypes();
                        _modelTypes = types?.Where(x => x.IsClass && x.GetInterface(typeof(IEFEntity).FullName) != null
                       && !x.IsGenericType && !x.IsAbstract
                       ).ToList();
                    }
                    else
                    {
                        _modelTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                            .Where(x => x.IsClass &&
                                  x.GetInterface(typeof(IEFEntity).FullName) != null &&
                                  !x.IsGenericType && !x.IsAbstract
                                  ).ToList();
                    }
                }
                return _modelTypes;
            }
        }

        EFOptions _options;
        public EFUnitOfWorkDbContext(EFOptions options, Action<LogLevel, string> logAction) : base(new DbContextOptions<EFUnitOfWorkDbContext>())
        {
            _options = options;
            _logAction = logAction;
            _loggerFactory = new LoggerFactory();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrEmpty(_options.EFProviderName))
            {
                throw new EFUnitOfWorkException("EFOptions.EFProviderName cannot be null or empty, please specify the EFProvider");
            }

            if (_options.OpenLogTrace && _logAction != null)
            {
                var provider = new SqlLogProvider(_logAction, _options.LogLevel);
                _loggerFactory.AddProvider(provider);
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseLoggerFactory(_loggerFactory);
            }

            var efProvider = EFProviderContainer.Instance.GetFactory(_options.EFProviderName);

            if (efProvider == null)
            {
                throw new EFUnitOfWorkException($"the EFOptions.EFProviderName:  {_options.EFProviderName}, not Register in EFProviderContainer");
            }

            efProvider.Use(optionsBuilder, _options.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddEntityTypes(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void AddEntityTypes(ModelBuilder modelBuilder)
        {
            ModelTypes.ForEach(x =>
            {
                if (modelBuilder.Model.FindEntityType(x) == null)
                    //modelBuilder.Model.AddEntityType(x);
                    //modelBuilder.Entity(x).HasNoKey();
                    modelBuilder.Entity(x);
            });
        }
    }
}
