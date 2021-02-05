using Jimu.Bus;

namespace IService.User.dto
{
    public class UserAdded : IJimuEvent
    {
        public string UserName { get; set; }
    }
}
