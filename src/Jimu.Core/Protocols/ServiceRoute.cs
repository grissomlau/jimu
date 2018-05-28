using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Jimu.Core.Protocols
{
    public class ServiceRoute
    {
        public IEnumerable<Address> Address { get; set; }

        public ServiceDescriptor ServiceDescriptor { get; set; }

        public static string ParseRoutePath(string routeTemplet, string service, string method,
            ParameterInfo[] parameterInfos)
        {
            var result = new StringBuilder();
            var parameters = routeTemplet.Split('/');
            foreach (var parameter in parameters)
            {
                var param = GetParameters(parameter).FirstOrDefault();
                if (param == null)
                    result.Append($"{parameter}/");
                else if (service.EndsWith(param))
                    result.Append($"{service.TrimStart('I').Substring(0, service.Length - param.Length - 1)}/");
                //else if (param == "Method")
                //{
                //    result.Append(method);
                //}
                //result.Append("/");
            }

            result.Append(method);
            result = new StringBuilder(result.ToString().ToLower());

            if (!parameterInfos.Any())
                return result.ToString().TrimEnd('&');

            result.Append("?");
            foreach (var para in parameterInfos)
                //if (para.IsOptional)
                //{
                //    result.Append($"[{para.Name}]=&");
                //}
                //else
                //{
                result.Append($"{para.Name}=&");
            //}

            return result.ToString().TrimEnd('&');
        }

        private static List<string> GetParameters(string text)
        {
            var matchVale = new List<string>();
            var reg = @"(?<={)[^{}]*(?=})";
            var key = string.Empty;
            foreach (Match m in Regex.Matches(text, reg)) matchVale.Add(m.Value);
            return matchVale;
        }
    }
}