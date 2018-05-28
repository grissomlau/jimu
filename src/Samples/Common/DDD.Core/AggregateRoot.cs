using System;
using System.Collections.Generic;
using System.Linq;

namespace DDD.Core
{
    public class AggregateRoot<TKey> :
        InlineEventHandler,
        IAggregateRoot<TKey>,
        IPurgeable
        where TKey : IEquatable<TKey>
    {
        private readonly Queue<IDomainEvent> _uncommittedEvents = new Queue<IDomainEvent>();
        public IEnumerable<IDomainEvent> UncommittedEvents => _uncommittedEvents;

        public TKey Id { get; protected set; }

        void IPurgeable.Purge()
        {
            if (_uncommittedEvents.Any())
            {
                _uncommittedEvents.Clear();
            }
        }

        public void Replay(IEnumerable<IDomainEvent> events)
        {
            ((IPurgeable)this).Purge();
            foreach (var e in events)
            {
                ApplyEvent(e);
            }
        }

        protected override void ApplyEvent<TEvent>(TEvent e)
        {
            //var eventHandlerMethods = from m in this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            //                          let parameters = m.GetParameters()
            //                          where m.IsDefined(typeof(InlineEventHandlerAttribute)) &&
            //                          m.ReturnType == typeof(void) &&
            //                          parameters.Length == 1 &&
            //                          parameters[0].ParameterType == e.GetType()
            //                          select m;
            ////e.AggregateRootType = this.GetType().FullName;
            //foreach (var handler in eventHandlerMethods)
            //{
            //    handler.Invoke(this, new object[] { e });
            //}
            base.ApplyEvent(e);
            _uncommittedEvents.Enqueue(e);
        }
    }
}
