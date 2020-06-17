using Microsoft.EntityFrameworkCore;

namespace Jimu.Server.UnitOfWork.EF
{
    public interface IEFProvider
    {
        void Use(DbContextOptionsBuilder optionsBuilder, string connectionString);
    }
}
