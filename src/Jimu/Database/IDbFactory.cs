using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Jimu.Database
{
    public interface IDbFactory
    {
        DbConnection Create();
    }
}
