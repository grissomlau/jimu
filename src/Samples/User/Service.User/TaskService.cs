using System.Collections.Generic;
using System.Threading.Tasks;
using IService.User;
using IService.User.dto;

namespace Service.User
{
    public class TaskService : ITaskService
    {
        public Task<List<UserModel>> GetListAsync()
        {
            return Task.FromResult(new List<UserModel> {
                new UserModel{  Id = 1, Name = "test"}
            });
        }

        public Task<UserModel> GetObjectAsync(int id)
        {
            return Task.FromResult(new UserModel
            {
                Id = 1,
                Name = "test"
            });
        }

        public Task<string> GetStringAsync(string anything)
        {
            return Task.FromResult(anything);
        }

        public Task GetVoidAsync()
        {
            return Task.CompletedTask;
        }
    }
}
