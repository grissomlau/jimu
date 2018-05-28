using DDD.Core;

namespace DDD.Simple.Domain.Events
{
    public class UserRegistered : DomainEvent
    {
        public string Email { get; }
        public string Name { get; }
        public UserRegistered(object aggregateRootKey, string name, string email) : base(aggregateRootKey)
        {
            Name = name;
            Email = email;
        }
    }
}
