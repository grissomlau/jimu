using System;

namespace Jimu.DDD
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class InlineEventHandlerAttribute : Attribute
    {
    }
}
