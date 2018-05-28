using System;

namespace DDD.Simple.IServices.DTO
{
    public class UserNameChangeReq : BaseReq
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }
}
