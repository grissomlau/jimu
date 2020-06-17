using Jimu.UnitOfWork;
using SqlSugar;
using System;
using System.Linq;

namespace Jimu.Server.UnitOfWork.SqlSugar
{
    public class DbFactory : IDbFactory<SqlSugarClient>
    {
        SqlSugarOptions _options;
        readonly Action<string> _logAction;
        public DbFactory(SqlSugarOptions options, Action<string> logAction)
        {
            _options = options;
            _logAction = logAction;
        }

        MultipleSqlSugarOptions _multipleOptions;
        public DbFactory(MultipleSqlSugarOptions multipleOptions, Action<string> logAction)
        {
            _multipleOptions = multipleOptions;
            _options = _multipleOptions.FirstOrDefault(x => x.IsDefaultOption);
            _logAction = logAction;
        }

        public SqlSugarClient Create(string optionName = null)
        {
            return this.Create(out SqlSugarOptions options, optionName);
        }
        public SqlSugarClient Create(out SqlSugarOptions options, string optionName = null)
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
                throw new SqlSugarUnitOfWorkException("Options is null");
            }
            var client = new SqlSugarClient(new ConnectionConfig
            {
                IsAutoCloseConnection = true,
                ConnectionString = options.ConnectionString,
                DbType = options.DbType,
                InitKeyType = InitKeyType.SystemTable
            });
            if (options.OpenLogTrace && _logAction != null)
            {
                client.Aop.OnLogExecuting = (sql, pars) =>
                {
                    var paraStr = "";
                    if (pars != null && pars.Any())
                    {
                        paraStr = Environment.NewLine + ", parameters: " + string.Join(Environment.NewLine + ",", pars.Select(x => $"DbType: {x.DbType} - Name: { x.ParameterName} - Value: { x.Value}"));
                    }
                    _logAction($"{sql}{paraStr}");
                    //Console.WriteLine($"SQL: {sql}");
                };
            }
            return client;
        }
    }
}
