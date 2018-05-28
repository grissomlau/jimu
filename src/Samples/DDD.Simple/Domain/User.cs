using System;
using System.Collections.Generic;
using DDD.Core;
using DDD.Simple.Domain.Events;

namespace DDD.Simple.Domain
{
    public class User : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string Email { get; private set; }

        public List<Guid> Friends { get; private set; }

        public static User Load(Guid id, string name, string email, List<Guid> friends)
        {
            var user = new User(id, name, email, friends);
            return user;
        }


        private User(Guid id, string name, string email, List<Guid> friends)
        {
            Id = id;
            Name = name;
            Email = email;
            Friends = friends;

        }
        public User(Guid id, string name, string email)
        {
            ApplyEvent(new UserRegistered(id, name, email));
        }

        public void ChangeName(string name)
        {
            ApplyEvent(new UserNameChanged(Id, name));
        }

        [InlineEventHandler]
        private void Handle(UserRegistered userRegistered)
        {
            Id = (Guid)userRegistered.AggregateRootKey;
            Name = userRegistered.Name;
            Email = userRegistered.Email;
        }

        [InlineEventHandler]
        private void Handle(UserNameChanged userNameChanged)
        {
            Name = userNameChanged.Name;
        }
    }
}
