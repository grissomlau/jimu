using IService.User;
using IService.User.dto;

namespace Service.User
{
    public class RestService : IRestService
    {
        public int Delete(int id)
        {
            return id;
        }

        public int Get(int id)
        {
            return id;
        }

        public string Get(string name, string email)
        {
            return $"name: {name}, email: {email}";
        }

        public string Post(UserModel user)
        {
            return $"name: {user.Name}";
        }

        public string Address(int id)
        {
            return $"userid: {id}, address: guangzhou";
        }

        public string Get(int id, string name)
        {
            return $"userid: {id}, name: {name}";
        }
    }
}
