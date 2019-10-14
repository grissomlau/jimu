using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using User.IService;
using User.IService.dto;

namespace User.Service
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
