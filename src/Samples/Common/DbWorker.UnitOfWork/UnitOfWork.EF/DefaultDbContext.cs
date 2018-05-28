using System.Linq;
using System.Reflection;
using DDD.Core;
using DbWorker.IUnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace DbWorker.UnitOfWork.EF
{
    public class DefaultDbContext : DbContext
    {
        private readonly DbContextOption _option;

        public DefaultDbContext(DbContextOption option) : base(new DbContextOptions<DefaultDbContext>())
        {
            _option = option;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_option.DbType)
            {
                case DbType.MySQL:
                    optionsBuilder.UseMySql(_option.ConnectionString);
                    break;
                default:
                    optionsBuilder.UseMySql(_option.ConnectionString);
                    break;
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AddEntityTypes(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void AddEntityTypes(ModelBuilder modelBuilder)
        {
            var assembly = Assembly.Load(_option.ModelAssemblyName);
            var types = assembly?.GetTypes();
            var list = types?.Where(x => x.IsClass && x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDbModel<>))
            && !x.IsGenericType && !x.IsAbstract
            ).ToList();
            if (list != null && list.Any())
            {
                list.ForEach(x =>
                {
                    if (modelBuilder.Model.FindEntityType(x) == null)
                        modelBuilder.Model.AddEntityType(x);
                });
            }
        }

    }
}
