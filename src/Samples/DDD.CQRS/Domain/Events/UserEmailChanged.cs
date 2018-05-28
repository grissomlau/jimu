using DDD.Core;

namespace DDD.CQRS.Domain.Events
{
    public class UserEmailChanged : DomainEvent
    {
        public string Email { get; set; }
        public UserEmailChanged(object aggregateRootKey, string email) : base(aggregateRootKey)
        {
            Email = email;
        }
    }
}
