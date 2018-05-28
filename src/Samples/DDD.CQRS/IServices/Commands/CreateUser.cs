using System;

namespace DDD.CQRS.IServices.Commands
{
    public class CreateUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
