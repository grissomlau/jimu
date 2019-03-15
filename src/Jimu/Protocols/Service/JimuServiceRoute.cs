using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Jimu
{
    public class JimuServiceRoute
    {
        public List<JimuAddress> Address { get; set; }

        public JimuServiceDesc ServiceDescriptor { get; set; }

        public static string ParseRoutePath(string routeTemplet, string service, string method,
            string[] paraNames, bool isInterface)
        {
            var result = new StringBuilder();
            var parameters = routeTemplet?.Split('/');// "/api/{ServiceName}" or "/api/UserService"
            foreach (var parameter in parameters)
            {
                var param = GetParameters(parameter).FirstOrDefault();
                if (param == null)
                    result.Append($"{parameter}/");
                else if (service.EndsWith(param))
                {
                    var curService = isInterface ? service.TrimStart('I') : service;
                    curService = curService.Substring(0, curService.Length - param.Length);
                    result.Append($"{curService}/");
                }
                //else if (param == "Method")
                //{
                //    result.Append(method);
                //}
                //result.Append("/");
            }

            result.Append(method);
            result = new StringBuilder(result.ToString().ToLower());

            if (!paraNames.Any())
                return result.ToString().TrimEnd('&', '/', '\\').TrimStart('/', '\\');

            result.Append("?");
            foreach (var para in paraNames)
                //if (para.IsOptional)
                //{
                //    result.Append($"[{para.Name}]=&");
                //}
                //else
                //{
                result.Append($"{para}=&");
            //}

            return result.ToString().TrimEnd('&', '/', '\\').TrimStart('/', '\\');
        }

        private static List<string> GetParameters(string text)
        {
            var matchVale = new List<string>();
            var reg = @"(?<={)[^{}]*(?=})";//{ServiceName}
            var key = string.Empty;
            foreach (Match m in Regex.Matches(text, reg)) matchVale.Add(m.Value);
            return matchVale;
        }
    }
}