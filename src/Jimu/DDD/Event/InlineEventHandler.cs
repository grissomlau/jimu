using System.Linq;
using System.Reflection;

namespace Jimu.DDD
{
    public abstract class InlineEventHandler
    {

        protected virtual void ApplyEvent<TEvent>(TEvent e) where TEvent : IDomainEvent
        {
            var eventHandlerMethods = from m in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                      let parameters = m.GetParameters()
                                      where m.IsDefined(typeof(InlineEventHandlerAttribute)) &&
                                      m.ReturnType == typeof(void) &&
                                      parameters.Length == 1 &&
                                      parameters[0].ParameterType == e.GetType()
                                      select m;
            //e.AggregateRootType = this.GetType().FullName;
            foreach (var handler in eventHandlerMethods)
            {
                handler.Invoke(this, new object[] { e });
            }
        }

    }
}
