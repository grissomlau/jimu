using DDD.Core;

namespace DDD.Simple.Domain.Events
{
    public class UserNameChanged : DomainEvent
    {
        public string Name { get; set; }
        public UserNameChanged(object aggregateRootKey, string name) : base(aggregateRootKey)
        {
            Name = name;
        }
    }
}
