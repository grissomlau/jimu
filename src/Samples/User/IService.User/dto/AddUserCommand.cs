using Jimu.Core.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace IService.User.dto
{
    public class AddUserCommand : IJimuCommand
    {
        public string UserName { get; set; }

        public string QueueName => "jimu-command-queue";
    }
}
