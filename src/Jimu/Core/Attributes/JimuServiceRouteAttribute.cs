using System;

namespace Jimu
{
    /// <summary>
    ///     set the service route path
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class JimuAttribute : Attribute
    {
        private string _routeTemplate;
        public JimuAttribute(string routeTemplate = null)
        {
            _routeTemplate = routeTemplate;
        }
        public string GetTemplate(Type type)
        {
            if (string.IsNullOrEmpty(_routeTemplate))
            {
                var arr = type.FullName.Split(".");
                if (arr.Length > 1)
                {
                    foreach (var name in arr)
                    {
                        if (name.StartsWith("IService") || name.StartsWith("Service"))
                            continue;
                        _routeTemplate += $"/{name}";
                    }
                    _routeTemplate += "{Service}";
                }
            }
            return _routeTemplate;
        }

    }
}