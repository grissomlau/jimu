using DDD.Core;

namespace DDD.CQRS.Domain.Events
{
    public class UserCreated : DomainEvent
    {
        public UserCreated(object aggregateRootKey, string name, string email) : base(aggregateRootKey)
        {
            Name = name;
            Email = email;
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
