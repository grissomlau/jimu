using System;
using DDD.Core;
using DDD.CQRS.Domain.Events;

namespace DDD.CQRS.Domain
{
    public class User : AggregateRoot<Guid>
    {
        public User(Guid id, string name, string email)
        {
            ApplyEvent(new UserCreated(id, name, email));
        }

        public string Name { get; private set; }
        public string Email { get; private set; }

        [InlineEventHandler]
        public void HandleUserCreated(UserCreated e)
        {
            Id = e.Id;
            Name = e.Name;
            Email = e.Email;
        }

        [InlineEventHandler]
        public void HandleEmailChanged(UserEmailChanged e)
        {
            Email = e.Email;
        }
    }
}
