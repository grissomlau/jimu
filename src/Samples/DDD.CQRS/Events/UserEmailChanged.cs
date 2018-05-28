using Jimu.DDD;
using System;
using System.Collections.Generic;
using System.Text;

namespace User.DDD.CQRS.Events
{
    public class UserEmailChanged : DomainEvent
    {
        public string Email { get; set; }
        public UserEmailChanged(object aggregateRootKey, string email) : base(aggregateRootKey)
        {
            this.Email = email;
        }
    }
}
