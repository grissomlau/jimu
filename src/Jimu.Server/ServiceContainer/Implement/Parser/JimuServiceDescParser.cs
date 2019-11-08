using Jimu.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace Jimu.Server.ServiceContainer.Implement.Parser
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
                    var setPath = string.IsNullOrEmpty(desc.Rest) ? methodInfo.Name : desc.Rest;
                    desc.RoutePath = JimuServiceRoute.ParseRoutePath(desc.HttpMethod, routeTemplate.RouteTemplate, type.Name, setPath, methodInfo.GetParameters().Select(x => x.Name).ToArray(), type.IsInterface);
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
                desc.ReturnFormat = jimuReturnComment.Default;
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

            Type returnType = methodInfo.ReturnType;

            if (methodInfo.ReturnType.ToString().IndexOf("System.Threading.Tasks.Task", StringComparison.Ordinal) == 0)
            {
                if (methodInfo.ReturnType.GenericTypeArguments.Any())
                {
                    returnType = methodInfo.ReturnType.GenericTypeArguments[0];
                }
                else
                {
                    returnType = typeof(void);
                }
            }
            desc.ReturnType = returnType.ToString();
            if (returnType.IsGenericType && returnType.ToString().StartsWith("System."))
            {
                returnType = returnType.GenericTypeArguments[0];
            }
            else if (returnType.IsArray)
            {
                var arrayBaseType = returnType.Assembly.ExportedTypes.FirstOrDefault(x => x.FullName == returnType.FullName.TrimEnd('[', ']'));
                returnType = arrayBaseType;
            }

            if (returnType != null)
            {
                desc.Properties = GetProperties(returnType);

            }
            return JimuHelper.Serialize<string>(desc);
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
                var paraComment = paraComments.FirstOrDefault(x => x.FieldName == para.Name);
                var paraDesc = new JimuServiceParameterDesc
                {
                    Name = para.Name,
                    Type = para.ParameterType.ToString(),
                    Comment = paraComment?.Comment,
                    Default = paraComment?.Default
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
                    paraDesc.Properties = GetProperties(para.ParameterType);
                }
                paras.Add(paraDesc);
            }
            return paras;
        }

        private List<JimuServiceParameterDesc> GetProperties(Type customType, int level = 0, string specifyName = null)
        {
            if (level > 3 || customType == null)
                return null;
            level++;
            StringBuilder sb = new StringBuilder(); ;
            var xmlNode = GetXmlComment(customType);

            List<JimuServiceParameterDesc> list = new List<JimuServiceParameterDesc>();
            if (customType.IsArray)
            {
                var arrayBaseType = customType.Assembly.ExportedTypes.FirstOrDefault(x => x.FullName == customType.FullName.TrimEnd('[', ']'));
                return new List<JimuServiceParameterDesc> {
                            new JimuServiceParameterDesc
                            {
                                Name = specifyName?? customType.Name,
                                //Type = customType.ToString().TrimEnd('[',']')
                                Type = customType.ToString(),
                                Properties = GetProperties(arrayBaseType)
                            }
                        };
            }
            else if (customType.IsEnum)
            {

                return new List<JimuServiceParameterDesc>{new JimuServiceParameterDesc
                    {
                        Name = specifyName?? customType.Name,
                        Type = "System.Int",

                    } };
            }
            else if (!customType.GetProperties().Any() || (customType.FullName.StartsWith("System.") && !customType.FullName.StartsWith("System.Collections")))
            {
                if (customType.FullName.StartsWith("System."))
                {

                    return new List<JimuServiceParameterDesc>{new JimuServiceParameterDesc
                    {
                        Name = specifyName?? customType.Name,
                        Type = customType.ToString(),

                    } };
                }
                return null;
            }

            foreach (var prop in customType.GetProperties())
            {
                var comment = prop.GetCustomAttribute<JimuFieldCommentAttribute>();
                //var proComment = comment == null ? "" : (" | " + comment?.Comment);
                var proComment = comment == null ? "" : ("" + comment?.Comment);
                var key = XmlCommentsMemberNameHelper.GetMemberNameForMember(prop);
                if (comment == null && xmlNode != null && xmlNode.TryGetValue(key, out var node))
                {
                    //proComment = $" | " + node.Value.Trim();
                    proComment = node.Value.Trim();
                }
                proComment = FilterJson(proComment);

                var paraDesc = new JimuServiceParameterDesc
                {
                    Comment = proComment,
                    Name = prop.Name,
                    Type = prop.PropertyType.ToString(),

                };
                if (prop.PropertyType.IsArray)
                {
                    paraDesc.Properties =
                        new List<JimuServiceParameterDesc> {
                            new JimuServiceParameterDesc
                            {
                                Type = prop.PropertyType.ToString().TrimEnd('[',']')
                            }
                        };
                }

                if (prop.PropertyType.IsClass
              && !prop.PropertyType.FullName.StartsWith("System."))
                {
                    paraDesc.Properties = GetProperties(prop.PropertyType, level);
                }
                else if (prop.PropertyType.IsClass
                    && prop.PropertyType.FullName.StartsWith("System.Collections.Generic.List")
                    && prop.PropertyType.GenericTypeArguments.Length == 1
                    )
                {
                    paraDesc.Properties = GetProperties(prop.PropertyType.GenericTypeArguments[0], level);
                }
                else if (prop.PropertyType.IsClass
                   && prop.PropertyType.FullName.StartsWith("System.Collections.Generic.Dictionary"))
                {
                    var keyPros = GetProperties(prop.PropertyType.GenericTypeArguments[0], level, "key");
                    var valPros = GetProperties(prop.PropertyType.GenericTypeArguments[1], level, "value");
                    if (valPros.Count > 1)
                    {
                        keyPros.Add(new JimuServiceParameterDesc
                        {
                            Name = "value",
                            Properties = valPros
                        });
                    }
                    else
                    {
                        keyPros.AddRange(valPros);
                    }
                    paraDesc.Properties = keyPros;
                }
                list.Add(paraDesc);
            }
            return list;

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
                var comment = prop.GetCustomAttribute<JimuFieldCommentAttribute>();
                var proComment = comment == null ? "" : (" | " + comment?.Comment);
                var key = XmlCommentsMemberNameHelper.GetMemberNameForMember(prop);
                if (comment == null && xmlNode != null && xmlNode.TryGetValue(key, out var node))
                {
                    proComment = $" | " + node.Value.Trim();
                }
                proComment = FilterJson(proComment);

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
                        sb.Append($"\"{prop.Name}\":\"[{prop.PropertyType.GenericTypeArguments[0].FullName}]\",");
                    }
                    else
                    {

                        sb.Append($"\"{prop.Name}\":[{{{GetCustomTypeMembers(prop.PropertyType.GenericTypeArguments[0], level)}}}],");
                    }
                }
                else if (prop.PropertyType.IsClass
                   && prop.PropertyType.FullName.StartsWith("System.Collections.Generic.Dictionary"))
                {

                    sb.Append($"\"{prop.Name}\":\"{prop.PropertyType.ToString().Replace("System.Collections.Generic.Dictionary`2", "")}{proComment}\",");
                }
                else
                {

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
            if (type == null)
                return new Dictionary<string, XPathNavigator>();

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
