using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace Jimu.Server.Implement.Parser
{
    class JimuServiceDescParser
    {
        private const string AllMemberXPath = "/doc/members";
        private const string MemberXPath = "/doc/members/member[@name='{0}']";
        private Dictionary<string, Dictionary<string, XPathNavigator>> _xmlComments;

        public JimuServiceDescParser()
        {
            _xmlComments = new Dictionary<string, Dictionary<string, XPathNavigator>>();
        }

        public JimuServiceDesc Parse(MethodInfo methodInfo)
        {
            JimuServiceDesc desc = new JimuServiceDesc();
            var descriptorAttributes = methodInfo.GetCustomAttributes<JimuServiceDescAttribute>();
            foreach (var attr in descriptorAttributes) attr.Apply(desc);

            if (string.IsNullOrEmpty(desc.Comment))
            {
                var xml = GetXmlComment(methodInfo.DeclaringType);
                var key = XmlCommentsMemberNameHelper.GetMemberNameForMethod(methodInfo);
                if (xml != null && xml.TryGetValue(key, out var node))
                {
                    var summaryNode = node.SelectSingleNode("summary");
                    if (summaryNode != null)
                        desc.Comment = summaryNode.Value.Trim();
                }
            }

            desc.ReturnDesc = GetReturnDesc(methodInfo);

            if (string.IsNullOrEmpty(desc.HttpMethod))
                desc.HttpMethod = GetHttpMethod(methodInfo);

            desc.Parameters = JimuHelper.Serialize<string>(GetParameters(methodInfo));

            if (string.IsNullOrEmpty(desc.Id))
            {
                desc.Id = JimuHelper.GenerateServiceId(methodInfo);
            }

            var type = methodInfo.DeclaringType;
            var routeTemplate = type.GetCustomAttribute<JimuServiceRouteAttribute>();
            if (routeTemplate != null)
            {

                if (string.IsNullOrEmpty(desc.RoutePath))
                {
                    var setPath = methodInfo.Name;
                    if (!string.IsNullOrEmpty(desc.RestPath))
                    {
                        setPath = desc.RestPath;
                    }
                    desc.RoutePath = JimuServiceRoute.ParseRoutePath(desc.HttpMethod,routeTemplate.RouteTemplate, type.Name, setPath, methodInfo.GetParameters().Select(x => x.Name).ToArray(), type.IsInterface);
                    if (!string.IsNullOrEmpty(desc.RestPath))
                    {
                        desc.RestPath = desc.RoutePath;
                    }
                }
            }

            desc.Service = methodInfo.DeclaringType.FullName;
            desc.ServiceComment = GetServiceComment(methodInfo);
            return desc;
        }

        private string GetServiceComment(MethodInfo methodInfo)
        {

            var xml = GetXmlComment(methodInfo.DeclaringType);
            var key = XmlCommentsMemberNameHelper.GetMemberNameForType(methodInfo.DeclaringType);
            if (xml != null && xml.TryGetValue(key, out var node))
            {
                var returnNode = node.SelectSingleNode("summary");
                return returnNode?.Value.Trim();
            }
            return null;

        }
        private string GetReturnDesc(MethodInfo methodInfo)
        {
            JimuServiceReturnDesc desc = new JimuServiceReturnDesc();
            var jimuReturnComment = methodInfo.GetCustomAttribute<JimuReturnCommentAttribute>();
            if (jimuReturnComment != null)
            {
                desc.Comment = jimuReturnComment.Comment;
                desc.ReturnFormat = jimuReturnComment.Format;
            }
            if (string.IsNullOrEmpty(desc.Comment))
            {
                var xml = GetXmlComment(methodInfo.DeclaringType);
                var key = XmlCommentsMemberNameHelper.GetMemberNameForMethod(methodInfo);
                if (xml != null && xml.TryGetValue(key, out var node))
                {
                    var returnNode = node.SelectSingleNode("returns");
                    desc.Comment = returnNode?.Value.Trim();
                }

            }

            List<Type> customTypes = new List<Type>();
            if (methodInfo.ReturnType.ToString().IndexOf("System.Threading.Tasks.Task", StringComparison.Ordinal) == 0 &&
                      methodInfo.ReturnType.IsGenericType)
            {
                //desc.ReturnType = methodInfo.ReturnType.ToString();
                desc.ReturnType = string.Join(",", methodInfo.ReturnType.GenericTypeArguments.Select(x => x.FullName));
                customTypes = (from type in methodInfo.ReturnType.GenericTypeArguments
                               from childType in type.GenericTypeArguments
                               select childType).ToList();
            }
            else if (methodInfo.ReturnType.IsGenericType)
            //if (methodInfo.ReturnType.IsGenericType)
            {
                //desc.ReturnType = string.Join(",", methodInfo.ReturnType.GenericTypeArguments.Select(x => x.FullName));
                desc.ReturnType = methodInfo.ReturnType.ToString();
                if (desc.ReturnType.StartsWith("System."))
                    customTypes = methodInfo.ReturnType.GenericTypeArguments.ToList();
                else
                    customTypes = new List<Type> { methodInfo.ReturnType };
            }
            else
            {
                desc.ReturnType = methodInfo.ReturnType.ToString();
                customTypes = new List<Type> { methodInfo.ReturnType };
            }

            // if not specify the return format in method attribute, we auto generate it.
            if (string.IsNullOrEmpty(desc.ReturnFormat))
            {
                desc.ReturnFormat = GetReturnFormat(customTypes);
            }
            return JimuHelper.Serialize<string>(desc);
        }

        private string GetReturnFormat(List<Type> types)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var customType in types)
            {
                if (customType.IsClass
                 && !customType.FullName.StartsWith("System."))
                {
                    sb.Append($"{{{ GetCustomTypeMembers(customType)}}},");
                }
                //else if (customType.FullName.StartsWith("System.Collections.Generic"))
                //{
                //    var childTypes = customType.GenericTypeArguments.ToList();
                //    sb.Append($"[{GetReturnFormat(childTypes)}]");
                //}
                else
                {
                    sb.Append($"{customType.ToString()},");
                }
            }
            return sb.ToString().TrimEnd(',');
        }

        private List<JimuServiceParameterDesc> GetParameters(MethodInfo method)
        {
            //StringBuilder sb = new StringBuilder();
            var paras = new List<JimuServiceParameterDesc>();
            var paraComments = method.GetCustomAttributes<JimuFieldCommentAttribute>();
            var xml = GetXmlComment(method.DeclaringType);
            var key = XmlCommentsMemberNameHelper.GetMemberNameForMethod(method);
            var hasNode = false;
            XPathNavigator node = null;
            if (xml != null)
                hasNode = xml.TryGetValue(key, out node);

            foreach (var para in method.GetParameters())
            {
                var paraDesc = new JimuServiceParameterDesc
                {
                    Name = para.Name,
                    Type = para.ParameterType.ToString(),
                    Comment = paraComments.FirstOrDefault(x => x.FieldName == para.Name)?.Comment,
                };
                if (string.IsNullOrEmpty(paraDesc.Comment) && hasNode)
                {
                    var paraNode = node.SelectSingleNode($"param[@name='{para.Name}']");
                    if (paraNode != null)
                        paraDesc.Comment = paraNode.Value.Trim();
                }
                if (para.ParameterType.IsClass
                && !para.ParameterType.FullName.StartsWith("System."))
                {
                    //var t = Activator.CreateInstance(para.ParameterType);
                    //sb.Append($"\"{para.Name}\":{_serializer.Serialize<string>(t)},");
                    //sb.Append($"\"{para.Name}\":{{{GetCustomTypeMembers(para.ParameterType)}}},");
                    paraDesc.Format = $"{{{ GetCustomTypeMembers(para.ParameterType)}}}";
                }
                else
                {
                    paraDesc.Format = $"{para.ParameterType.ToString()}";
                    //sb.Append($"\"{para.Name}\":\"{para.ParameterType.ToString()}\",");
                }
                paras.Add(paraDesc);
            }
            //return "{" + sb.ToString().TrimEnd(',') + "}";
            return paras;
        }

        private string GetCustomTypeMembers(Type customType, int level = 0)
        {
            if (level > 3)
                return "";

            level++;
            StringBuilder sb = new StringBuilder(); ;
            var xmlNode = GetXmlComment(customType);
            foreach (var prop in customType.GetProperties())
            {
                if (prop.PropertyType.IsClass
              && !prop.PropertyType.FullName.StartsWith("System."))
                {

                    sb.Append($"\"{prop.Name}\":{{{GetCustomTypeMembers(prop.PropertyType, level)}}},");
                }
                else if (prop.PropertyType.IsClass
                    && prop.PropertyType.FullName.StartsWith("System.Collections.Generic.List")
                    && prop.PropertyType.GenericTypeArguments.Length == 1
                    )
                {
                    if (prop.PropertyType.GenericTypeArguments[0].FullName.StartsWith("System."))
                    {
                        sb.Append($"\"{prop.Name}\":[{{}}],");
                    }
                    else
                    {

                        sb.Append($"\"{prop.Name}\":[{{{GetCustomTypeMembers(prop.PropertyType.GenericTypeArguments[0], level)}}}],");
                    }
                }
                else
                {
                    var comment = prop.GetCustomAttribute<JimuFieldCommentAttribute>();
                    var proComment = comment == null ? "" : (" | " + comment?.Comment);
                    var key = XmlCommentsMemberNameHelper.GetMemberNameForMember(prop);
                    if (comment == null && xmlNode != null && xmlNode.TryGetValue(key, out var node))
                    {
                        proComment = $" | " + node.Value.Trim();
                    }
                    proComment = FilterJson(proComment);
                    sb.Append($"\"{prop.Name}\":\"{prop.PropertyType.ToString()}{proComment}\",");
                }
            }
            return sb.ToString().TrimEnd(',');
        }

        private static string FilterJson(string objJson)
        {
            if (string.IsNullOrEmpty(objJson)) return objJson;
            return objJson?.Replace(@"\", @"\\").Replace("\"", "&quot;");
        }

        private string GetHttpMethod(MethodInfo method)
        {
            if (method.Name.StartsWith("get", StringComparison.OrdinalIgnoreCase))
            {
                return "GET";
            }
            else
            {
                return method.GetParameters().Any(x => x.ParameterType.IsClass
                    && !x.ParameterType.FullName.StartsWith("System.")) ? "POST" : "GET";
            }
        }

        private Dictionary<string, XPathNavigator> GetXmlComment(Type type)
        {
            var key = type.Assembly.GetName().Name;
            //Dictionary<string, XPathNavigator> xmlComments = null;
            var fileFullName = $"{key}.xml";
            if (!File.Exists(fileFullName))
            {
                var assemblyPath = Path.GetDirectoryName(type.Assembly.Location);
                fileFullName = Path.Combine(assemblyPath + "", fileFullName);
            }
            if (!_xmlComments.TryGetValue(key, out var xmlComments)
                && File.Exists(fileFullName)
                )
            {
                using (var sr = File.OpenText(fileFullName))
                {
                    var xmlMembers = new XPathDocument(sr).CreateNavigator().SelectSingleNode(AllMemberXPath);
                    xmlComments = new Dictionary<string, XPathNavigator>();
                    foreach (XPathNavigator path in xmlMembers.Select("member"))
                    {
                        xmlComments.Add(path.GetAttribute("name", ""), path);
                    }
                    _xmlComments.Add(key, xmlComments);
                }
            }
            return xmlComments;
        }

    }
}
