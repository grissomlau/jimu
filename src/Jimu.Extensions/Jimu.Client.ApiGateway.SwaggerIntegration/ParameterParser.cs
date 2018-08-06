using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.SwaggerIntegration
{
    class ParameterParser
    {
        private readonly string _paramStr;
        private readonly List<JimuServiceParameterDesc> _paras = new List<JimuServiceParameterDesc>();
        private readonly string _httpMethod;
        private List<IParameter> _parameters = new List<IParameter>();
        public ParameterParser(List<JimuServiceParameterDesc> paras, string httpMethod)
        {
            _httpMethod = httpMethod;
            _paras = paras;

            int idx = 0;
            StringBuilder sbExample = new StringBuilder();
            foreach (var p in _paras)
            {
                idx++;
                //TypeInfo typeInfo = ParseType(kvParam.Type);
                if (_httpMethod == "GET")
                {
                    var param = new NonBodyParameter
                    {
                        Name = p.Name,
                        Type = p.Type,
                        //Format = typeInfo.Format,
                        In = "query",
                        Description = $"{p.Comment}",
                    };
                    //if (typeInfo.IsArray)
                    if (CheckIsArray(p.Type))
                    {
                        param.Format = null;
                        param.Items = new PartialSchema
                        {
                            //Type = typeInfo.Type
                            Type = GetArrayType(p.Type)
                        };
                        param.Type = "array";
                    }
                    _parameters.Add(param);
                }
                else
                {

                    var bodyPara = new BodyParameter
                    {
                        Name = p.Name,
                        In = "body",
                        Description = $"{p.Comment}",
                        Schema = new Schema
                        {
                            Format = p.Format,
                        }

                    };
                    // swagger bug: two or more object parameter in post, when execute it, just post the last one,so we put all parameter in the last one that it can post it
                    if (!string.IsNullOrEmpty(p.Format) && p.Format.IndexOf("{") < 0)
                    {
                        sbExample.Append($"{p.Name}:\"{ p.Format}\",");
                    }
                    else if (!string.IsNullOrEmpty(p.Format))
                    {

                        sbExample.Append($"{p.Name}:{ p.Format},");
                    }
                    if (idx == _paras.Count && sbExample.Length > 0 && _paras.Count > 1)
                    {
                        bodyPara.Schema.Example = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"{{{sbExample.ToString().TrimEnd(',')}}}");
                    }
                    else if (idx == _paras.Count && sbExample.Length > 0)
                    {
                        bodyPara.Schema.Example = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"{{{sbExample.ToString().TrimEnd(',')}}}");

                    }

                    _parameters.Add(bodyPara);
                }
            }
        }

        public List<IParameter> GetParameters()
        {
            return _parameters;
        }

        public static string GetArrayType(string type)
        {
            if (type.IndexOf("System.Collections") >= 0 || type.IndexOf("[]") >= 0)
            {
                if (type.IndexOf("System.Collections") >= 0)
                {
                    type = type.Substring(type.IndexOf('[') + 1).TrimEnd(']');
                }
                else if (type.IndexOf("[]") >= 0)
                {
                    type = type.Replace("[]", "");
                }
            }
            return type;
        }


        public static bool CheckIsArray(string type)
        {
            return type.IndexOf("System.Collections") >= 0 || type.IndexOf("[]") >= 0;
        }
        public static bool CheckIsObject(string type)
        {
            var systemTypeDic = GetSystemTypeDic();
            foreach (var sy in systemTypeDic)
            {
                if (type == sy.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public static Dictionary<string, string> GetSystemTypeDic()
        {
            Dictionary<string, string> systemTypeDic = new Dictionary<string, string>();
            systemTypeDic.Add("System.String", "string");
            systemTypeDic.Add("System.Int16", "integer");
            systemTypeDic.Add("System.UInt16", "integer");
            systemTypeDic.Add("System.Int32", "integer");
            systemTypeDic.Add("System.UInt32", "integer");
            systemTypeDic.Add("System.Int64", "integer");
            systemTypeDic.Add("System.UInt64", "integer");
            systemTypeDic.Add("System.Single", "number");
            systemTypeDic.Add("System.Double", "number");
            systemTypeDic.Add("System.Decimal", "number");
            systemTypeDic.Add("System.Byte", "string");
            systemTypeDic.Add("System.Boolean", "boolean");
            systemTypeDic.Add("System.DateTime", "datetime");
            return systemTypeDic;

        }

        public static string ReplaceTypeToJsType(string str)
        {
            foreach (var p in GetSystemTypeDic())
            {
                str = str.Replace(p.Key, p.Value);
            }
            return str;
        }

        public class TypeInfo
        {
            public string Type { get; set; }
            public string Format { get; set; }
            public string Desc { get; set; }
            public bool IsArray { get; set; }
        }
    }
}
