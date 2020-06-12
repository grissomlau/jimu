using Jimu.Core.Bus;

namespace IService.User.dto
{
    public class AddUserCommand : IJimuCommand
    {
        public string UserName { get; set; }

        public string QueueName => "jimu-command-queue";
    }
}
