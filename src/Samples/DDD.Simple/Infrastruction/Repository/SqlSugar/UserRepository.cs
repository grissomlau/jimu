using System;
using System.Linq;
using DDD.Core;
using DDD.Simple.Domain.Events;
using DDD.Simple.Model;
using DbWorker.IUnitOfWork;
using SqlSugar;
using User = DDD.Simple.Domain.User;

namespace DDD.Simple.Repository.SqlSugar
{
    public class UserRepository :
        Repository<User, Guid>
    {
        //BaseDao<Model.User, Guid> _userDao;
        //BaseDao<Model.UserFriend, Guid> _userFriendDao;

        Model.User _user;
        readonly SqlClient<SqlSugarClient> _sqlClient;
        readonly Guid id;
        public UserRepository(IUnitOfWork unitOfWork)
        {
            //_userDao = new BaseDao<Model.User, Guid>(unitOfWork);
            //_userFriendDao = new BaseDao<Model.UserFriend, Guid>(unitOfWork);
            _sqlClient = unitOfWork.GetSqlClient<SqlSugarClient>();
            id = Guid.NewGuid();
        }
        public override User Get(Guid key)
        {
            Console.WriteLine($"Repository guid is {id.ToString()}");
            var user = _sqlClient.Client.Queryable<Model.User>().Where(x => x.Id == key).Single();

            var friends = _sqlClient.Client.Queryable<UserFriend>().Where(x => x.UserId == user.Id).ToList();

            return User.Load(user.Id, user.Name, user.Email, friends.Select(x => x.Id).ToList());
        }
        public override void Save(User aggreateRoot)
        {
            base.Save(aggreateRoot);
            _user = null;
        }


        [InlineEventHandler]
        private void HandleUserRegistered(UserRegistered userRegistered)
        {
            var user = GetUserModel(userRegistered);
            user.Id = (Guid)userRegistered.AggregateRootKey;
            user.Name = userRegistered.Name;
            user.Email = userRegistered.Email;

            //_userDao.Insert(_user);
            _sqlClient.Client.Insertable(user).ExecuteCommand();
        }

        [InlineEventHandler]
        private void HandleUserNameChanged(UserNameChanged userNameChanged)
        {
            var user = GetUserModel(userNameChanged);
            user.Name = userNameChanged.Name;
            //_userDao.Update(_user);
            _sqlClient.Client.Updateable(user).UpdateColumns(x => new { x.Name }).ExecuteCommand();
        }

        private Model.User GetUserModel(IDomainEvent e)
        {
            if (_user == null)
            {
                _user = _sqlClient.Client.Queryable<Model.User>().Where(x => x.Id == (Guid)e.AggregateRootKey).Single();
            }

            return _user ?? (_user = new Model.User());
        }


    }
}
