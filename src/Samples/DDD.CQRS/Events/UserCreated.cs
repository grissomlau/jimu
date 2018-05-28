using Jimu.DDD;
using System;

namespace User.DDD.CQRS.Events
{
    public class UserCreated : DomainEvent
    {
        public UserCreated(object aggregateRootKey, string name, string email) : base(aggregateRootKey)
        {
            this.Name = name;
            this.Email = email;
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
