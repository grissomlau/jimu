using System.Threading.Tasks;
using Jimu;
using Simple.IServices;

namespace Simple.Services
{
    public class UserService : IUserService
    {

        private readonly ILogger _logger;
        public UserService(ILogger logger)
        {
            _logger = logger;
        }
        public string GetId()
        {
            return "grissom id3";
        }

        public Task<string> GetName()
        {
            return Task.FromResult("grissom name3");
        }

        public void SetId()
        {
            _logger.Info("set id success");
        }

        public async Task SetName(string name)
        {
            await Task.Run(() =>
           {
               _logger.Info("set name is " + name);
           });
        }
    }
}
