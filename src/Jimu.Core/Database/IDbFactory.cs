using System.Data.Common;

namespace Jimu.Database
{
    public interface IDbFactory
    {
        DbConnection Create(string name = null);
    }
}
