using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DDD.Core;
using DDD.Simple.Domain;
using DDD.Simple.IServices;
using DDD.Simple.IServices.DTO;
using DbWorker.IUnitOfWork;
using Jimu;

namespace DDD.Simple.Services
{
    public class UserService : IUserService
    {
        #region mongodb
        //private readonly IMongoDatabase _db;
        //public UserService(IMongoDatabase db)
        //{
        //    _db = db;
        //}
        //public async Task<Guid> CreateUser(UserCreateReq userCreateReq)
        //{
        //    var users = _db.GetCollection<User>("user");
        //    var user = new User(Guid.NewGuid(), userCreateReq.Name, userCreateReq.Email);
        //    //var filter = Builders<BsonDocument>.Filter.Eq("Id",)
        //    await users.InsertOneAsync(user);
        //    return user.Id;

        //}

        //public User GetUser(Guid id)
        //{
        //    var users = _db.GetCollection<User>("user");
        //    var filter = Builders<User>.Filter.Eq(x => x.Id, id);
        //    var user = users.Find(filter).FirstOrDefault();
        //    if (user != null)
        //    {
        //        return user;
        //    }
        //    return null;
        //}

        #endregion


        readonly IUnitOfWork _unitOfWork;
        readonly IRepository<User, Guid> _userRepository;
        readonly IRepository<Order, Guid> _orderRepository;
        ITypeConvertProvider _typeConvertProvider;
        ISerializer _serializer;
        readonly Guid _id;
        readonly JimuPayload _payload;
        public UserService(IUnitOfWork unitOfWork, IRepository<User, Guid> userRepository, IRepository<Order, Guid> orderRepository, JimuPayload payload, ITypeConvertProvider typeConvertProvider, ISerializer serializer)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _id = Guid.NewGuid();
            _payload = payload;
            _typeConvertProvider = typeConvertProvider;
            _serializer = serializer;
        }
        public Task<Guid> CreateUser(UserCreateReq userCreateReq)
        {
            _unitOfWork.BeginTran();
            try
            {
                var user = new User(Guid.NewGuid(), userCreateReq.Name, userCreateReq.Email);
                //_userRepository.Save(user);
                user.ChangeName("haha");
                _userRepository.Save(user);

                // order
                var order = new Order(100);
                _orderRepository.Save(order);
                _unitOfWork.CommitTran();
                return Task.FromResult(user.Id);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTran();
                throw ex;
            }
        }

        public Task<UserDto> GetByName(string userName)
        {
            return null;
        }

        public class Test
        {
            public string Id { get; set; }
            public byte[] Data { get; set; }
        }
        public async Task<JimuFile> GetFile()
        {
            //HttpResponseMessage msg = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            using (var ms = new MemoryStream())
            {
                using (var sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/files/test.xlsx")))
                {
                    sr.BaseStream.CopyTo(ms);
                    var bytes = await Task.FromResult(ms.ToArray());
                    //var dy = new Test { Id = "1", Data = bytes };
                    //var cb = _serializer.Serialize<string>(dy);
                    //var bb = (Test)_serializer.Deserialize<string>(cb, typeof(Test));
                    ////var reb = _serializer.Deserialize(bb.Data, typeof(byte[]));
                    //var reb = Encoding.UTF8.GetBytes("anVldCB0ZXN0");
                    return new JimuFile("test.xlsx", bytes);
                    //msg.Content = new StreamContent(sr.BaseStream);
                }
                //return msg;

            }
        }

        public UserDto GetUser(Guid id)
        {
            Console.WriteLine($"GetUser guid is {_id.ToString()}");
            Console.WriteLine($"GetUser payload username is {_payload.Items["username"]}");
            Console.WriteLine($"GetUser payload role is {_payload.Items["role"]}");
            var user = _userRepository.Get(id);
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }

        public Task<Guid> UpdateUser(UserNameChangeReq userNameChangeReq)
        {
            var user = _userRepository.Get(userNameChangeReq.UserId);
            user.ChangeName(userNameChangeReq.Name);
            _unitOfWork.BeginTran();
            try
            {
                _userRepository.Save(user);
                _unitOfWork.CommitTran();
                return Task.FromResult(user.Id);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTran();
                throw ex;
            }
        }

        public Task UploadFiles(List<JimuFile> files)
        {
            foreach (var file in files)
            {
                using (MemoryStream ms = new MemoryStream(file.Data))
                {
                    using (var sw = new StreamWriter("c://files/" + file.FileName, false))
                    {
                        ms.CopyTo(sw.BaseStream);
                    }
                }
            }
            Debug.WriteLine("files =======>" + files.Count);
            return Task.CompletedTask;
        }
    }
}
