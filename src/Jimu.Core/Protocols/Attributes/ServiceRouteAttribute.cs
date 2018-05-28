using System;

namespace Jimu.Core.Protocols.Attributes
{
    /// <summary>
    ///     set the service route path
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class ServiceRouteAttribute : Attribute
    {
        public ServiceRouteAttribute(string routeTemplate)
        {
            RouteTemplate = routeTemplate;
        }

        public string RouteTemplate { get; }
    }
}