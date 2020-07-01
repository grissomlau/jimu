using Jimu.Logger;
using System;
using System.Collections.Generic;
using IService.User;
using IService.User.dto;
using Dapper;
using System.Threading.Tasks;
using System.Data.Common;
using Jimu.UnitOfWork;

namespace Service.User
{
    public class MySqlService : IMySqlService
    {
        readonly ILogger _logger;
        readonly IDbFactory<DbConnection> _dbFactory;
        public MySqlService(ILoggerFactory loggerFactory, IDbFactory<DbConnection> dbFactory)
        {
            _logger = loggerFactory.Create(this.GetType());
            _dbFactory = dbFactory;
        }
        public int AddUser(UserModel user)
        {
            try
            {
                using (var db = _dbFactory.Create())
                {
                    return db.Execute("Insert Into user(`Name`,`Email`)Values(@Name, @Email)", user);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("adduser", ex);
                return 0;
            }
        }

        public List<UserModel> GetAllUser()
        {
            List<UserModel> users = new List<UserModel>();
            using (var db = _dbFactory.Create("RWDB"))
            {
                users = db.Query<UserModel>("Select * From user").AsList();
            }
            UserModel user = null;
            using (var db = _dbFactory.Create())
            {
                user = db.QuerySingleOrDefault<UserModel>("Select * From user Where Id = @id", new { id = 1 });
            }
            return users;
        }

        public Task<List<UserModel>> GetAllUserArray()
        {
            return Task.FromResult(new List<UserModel>());
        }

        public UserModel GetUser(int id)
        {
            using (var db = _dbFactory.Create())
            {
                return db.QuerySingleOrDefault<UserModel>("Select * From user Where Id = @id", new { id });
            }
        }

    }
}
