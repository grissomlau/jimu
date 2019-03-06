using System;

namespace Jimu
{
    /// <summary>
    ///     set the service route path
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class JimuServiceRouteAttribute : Attribute
    {
        public JimuServiceRouteAttribute(string routeTemplate)
        {
            RouteTemplate = routeTemplate;
        }

        public string RouteTemplate { get; }
    }
}