using Autofac;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jimu.Client.ApiGateway.SwaggerIntegration
{
    public class JimuSwaggerDocumentFilter : IDocumentFilter
    {
        ISerializer _serializer;
        public JimuSwaggerDocumentFilter(ISerializer serializer)
        {
            _serializer = serializer;
        }
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var serviceDiscovery = JimuClient.Host.Container.Resolve<IClientServiceDiscovery>();
            var routes = serviceDiscovery.GetRoutesAsync().GetAwaiter().GetResult();

            (from route in routes select route.ServiceDescriptor).ToList().ForEach(x =>
            {
                var subsIndex = x.RoutePath.IndexOf('?');
                subsIndex = subsIndex < 0 ? x.RoutePath.Length : subsIndex;
                var route = x.RoutePath.Substring(0, subsIndex);
                route = route.StartsWith('/') ? route : "/" + route;

                var paras = new List<IParameter>();
                if (!string.IsNullOrEmpty(x.Parameters))
                {
                    var parameters = _serializer.Deserialize(TypeHelper.ReplaceTypeToJsType(x.Parameters), typeof(List<JimuServiceParameterDesc>)) as List<JimuServiceParameterDesc>;
                    paras = GetParameters(parameters, x.HttpMethod);
                }

                if (x.GetMetadata<bool>("EnableAuthorization"))
                {
                    paras.Add(new NonBodyParameter
                    {
                        Name = "Authorization",
                        Type = "string",
                        In = "header",
                        Description = "Token",
                        Required = true,
                        Default = "Bearer "
                    });
                }

                var response = new Dictionary<string, Response>();
                response.Add("200", GetResponse(x.ReturnDesc));

                if (x.HttpMethod == "GET")
                {

                    swaggerDoc.Paths.Add(route, new PathItem
                    {
                        Get = new Operation
                        {
                            Consumes = new List<string> { "application/json" },
                            OperationId = x.RoutePath,
                            Parameters = paras,
                            Produces = new List<string> { "application/json" },
                            Responses = response,
                            Description = x.Comment
                        }
                    });
                }
                else
                {
                    swaggerDoc.Paths.Add(route, new PathItem
                    {
                        Post = new Operation
                        {
                            Consumes = new List<string> { "application/json" },
                            OperationId = x.RoutePath,
                            Parameters = paras,
                            Produces = new List<string> { "application/json" },
                            Responses = response,
                            Description = x.Comment
                        }
                    });
                }
            });

        }

        private static void AddAuthorization()
        {

        }

        private static Response GetResponse(string returnDescStr)
        {

            if (string.IsNullOrEmpty(returnDescStr) || !returnDescStr.StartsWith('{'))
            {
                return new Response
                {
                    Description = "Success",
                    Schema = new Schema
                    {
                        Type = returnDescStr
                    }
                };
            }
            var returnDesc = Newtonsoft.Json.JsonConvert.DeserializeObject<JimuServiceReturnDesc>(TypeHelper.ReplaceTypeToJsType(returnDescStr));
            var isObject = TypeHelper.CheckIsObject(returnDesc.ReturnType);
            var response = new Response
            {
                Description = string.IsNullOrEmpty(returnDesc.Comment) ? "Success" : returnDesc.Comment,
                Schema = new Schema
                {
                    Type = isObject ? "object" : returnDesc.ReturnType,
                    Example = (isObject && returnDesc.ReturnFormat.StartsWith('{')) ? Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(returnDesc.ReturnFormat) : returnDesc.ReturnFormat,
                }
            };
            var isArray = TypeHelper.CheckIsArray(returnDesc.ReturnType);
            if (isArray)
            {
                response.Schema.Example = (isObject && returnDesc.ReturnFormat.StartsWith('{')) ? Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"[{returnDesc.ReturnFormat}]") : $"[{returnDesc.ReturnFormat}]";
            }
            return response;
        }

        private static List<IParameter> GetParameters(List<JimuServiceParameterDesc> paras, string httpMethod)
        {
            List<IParameter> parameters = new List<IParameter>();
            int idx = 0;
            StringBuilder sbExample = new StringBuilder();
            foreach (var p in paras)
            {
                idx++;
                if (httpMethod == "GET")
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
                    if (TypeHelper.CheckIsArray(p.Type))
                    {
                        param.Format = null;
                        param.Items = new PartialSchema
                        {
                            //Type = typeInfo.Type
                            Type = TypeHelper.GetArrayType(p.Type)
                        };
                        param.Type = "array";
                    }
                    parameters.Add(param);
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
                    if (idx == paras.Count && sbExample.Length > 0 && paras.Count > 1)
                    {
                        bodyPara.Schema.Example = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"{{{sbExample.ToString().TrimEnd(',')}}}");
                    }
                    else if (idx == paras.Count && sbExample.Length > 0)
                    {
                        bodyPara.Schema.Example = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"{{{sbExample.ToString().TrimEnd(',')}}}");

                    }

                    parameters.Add(bodyPara);
                }
            }
            return parameters;
        }
    }


}
