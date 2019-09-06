using MiniDDD.UnitOfWork;
using System;

namespace Jimu.Server.Repository.MiniDDD.SqlSugar
{
    public class MiniDDDSqlSugarOptions
    {

        public string ConnectionString { get; set; }
        public DbType DbType { get; set; }

        /// <summary>
        /// enable logging sql, this will effect lower perfermance, always just using in debug
        /// </summary>
        public bool OpenLogTrace { get; set; }

    }
}
