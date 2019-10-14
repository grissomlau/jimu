using Jimu.Database;
using Jimu.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using User.IService;
using User.IService.dto;
using Dapper;
using System.Threading.Tasks;

namespace User.Service
{
    public class MySqlService : IMySqlService
    {
        readonly ILogger _logger;
        readonly IDbFactory _dbFactory;
        public MySqlService(ILogger logger, IDbFactory dbFactory)
        {
            _logger = logger;
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
            using (var db = _dbFactory.Create())
            {
                return db.Query<UserModel>("Select * From user").AsList();
            }
        }

        public Task<UserModel[]> GetAllUserArray()
        {
            return Task.FromResult(new UserModel[] { });
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
