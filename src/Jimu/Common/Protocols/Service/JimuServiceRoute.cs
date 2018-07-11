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
            ParameterInfo[] parameterInfos)
        {
            var result = new StringBuilder();
            var parameters = routeTemplet?.Split('/');
            foreach (var parameter in parameters)
            {
                var param = GetParameters(parameter).FirstOrDefault();
                if (param == null)
                    result.Append($"{parameter}/");
                else if (service.EndsWith(param))
                {
                    var curService = service.TrimStart('I');
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