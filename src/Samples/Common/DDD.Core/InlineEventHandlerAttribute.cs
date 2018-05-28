using System;

namespace DDD.Core
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InlineEventHandlerAttribute : Attribute
    {
    }
}
