using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jimu
{
    public class JimuServiceRoute
    {
        public List<JimuAddress> Address { get; set; }

        public JimuServiceDesc ServiceDescriptor { get; set; }

        //public static string ParseRoutePath(string routeTemplet, string service, string method,
        //    string[] paraNames, bool isInterface)
        //{
        //    var result = new StringBuilder();
        //    var parameters = routeTemplet?.Split('/');// "/api/{ServiceName}" or "/api/UserService"
        //    foreach (var parameter in parameters)
        //    {
        //        var param = GetParameters(parameter).FirstOrDefault();
        //        if (param == null)
        //            result.Append($"{parameter}/");
        //        else if (service.EndsWith(param))
        //        {
        //            var curService = isInterface ? service.TrimStart('I') : service;
        //            curService = curService.Substring(0, curService.Length - param.Length);
        //            result.Append($"{curService}/");
        //        }
        //        //else if (param == "Method")
        //        //{
        //        //    result.Append(method);
        //        //}
        //        //result.Append("/");
        //    }

        //    result.Append(method);
        //    result = new StringBuilder(result.ToString().ToLower());

        //    if (paraNames.Any())
        //    {
        //        //return result.ToString().TrimEnd('&', '/', '\\').TrimStart('/', '\\');
        //        //return "/" + result.ToString().TrimEnd('&', '/', '\\').TrimStart('/', '\\');

        //        result.Append("?");
        //        foreach (var para in paraNames)
        //            //if (para.IsOptional)
        //            //{
        //            //    result.Append($"[{para.Name}]=&");
        //            //}
        //            //else
        //            //{
        //            result.Append($"{para}={{{para}}}&");
        //        //}
        //    }

        //    //return result.ToString().TrimEnd('&', '/', '\\').TrimStart('/', '\\');
        //    return "/" + result.ToString().TrimEnd('&', '/', '\\').TrimStart('/', '\\');
        //}

        public static string ParseRoutePath(string httpMethod, string routeTemplet, string service, string method, string[] paraNames, bool isInterface)
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
                    var curService = service;
                    if (isInterface && service.StartsWith('I') && service.Length > 1)
                    {
                        curService = service.Substring(1);
                    }
                    curService = curService.Substring(0, curService.Length - param.Length);
                    result.Append($"{curService}/");
                }
            }

            result.Append(method.TrimStart('/'));
            //result = new StringBuilder(result.ToString().ToLower());
            result = new StringBuilder(lowerFirstLetter(result.ToString()));

            if (paraNames.Any()
                && (httpMethod.Equals("GET", System.StringComparison.OrdinalIgnoreCase)
                    || httpMethod.Equals("DELETE", System.StringComparison.OrdinalIgnoreCase)))
            {
                result.Append("?");
                foreach (var para in paraNames)
                {
                    if (method.IndexOf($"{{{para}}}") < 0)
                    {
                        result.Append($"{para}={{{para}}}&");
                    }
                }
            }
            return "/" + result.ToString().TrimEnd('?', '&', '/', '\\').TrimStart('/', '\\');
        }

        private static string lowerFirstLetter(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return path;
            var arr = path.Split('/');
            string newPath = "";
            foreach (var s in arr)
            {
                if (!string.IsNullOrWhiteSpace(s))
                    newPath += "/" + char.ToLower(s[0]) + s.Substring(1);
            }
            return newPath;
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