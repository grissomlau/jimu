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
                var jimuParas = new Dictionary<string, dynamic>();
                if (!string.IsNullOrEmpty(x.Parameters))
                {
                    var parameters = _serializer.Deserialize(ParameterParser.ReplaceTypeToJsType(x.Parameters), typeof(List<JimuServiceParameterDesc>)) as List<JimuServiceParameterDesc>;
                    paras = new ParameterParser(parameters, x.HttpMethod).GetParameters();
                }

                var response = new Dictionary<string, Response>();
                var returnType = GetType(x.ReturnType);
                //switch (x.ReturnType)
                //{
                //    case "System.String":
                //        response.Add("200", new Response
                //        {
                //            Description = "Success",
                //            Schema = new Schema
                //            {
                //                Type = returnType.Item1,
                //                Format = returnType.Item2,
                //            }

                //        });
                //        break;
                //}

                response.Add("200", new Response
                {
                    Description = "Success",
                    Schema = new Schema
                    {
                        Type = returnType.Item1,
                        Format = returnType.Item2,
                    }

                });

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
                            Responses = response
                        }
                    });
                }
                else
                {
                    //var idx = 0;
                    //foreach (var p in paras)
                    //{
                    //    Schema schema = null;
                    //    if (idx == jimuParas.Count - 1)
                    //    {
                    //        schema = new Schema
                    //        {
                    //            Example = jimuParas
                    //        };
                    //    }
                    //    paras.Add(
                    //        new BodyParameter
                    //        {
                    //            Name = p.Key,
                    //            Required = true,
                    //            In = "body",
                    //            Description = p.Value + "",
                    //            Schema = schema
                    //        }
                    //        );
                    //    idx++;
                    //}
                    swaggerDoc.Paths.Add(route, new PathItem
                    {
                        Post = new Operation
                        {
                            Consumes = new List<string> { "application/json" },
                            OperationId = x.RoutePath,
                            Parameters = paras,
                            Produces = new List<string> { "application/json" },
                            Responses = response
                        }
                    });
                }
            });

        }

        private static ValueTuple<string, string> GetType(string type)
        {
            string format = "";
            if (ParameterParser.CheckIsObject(type))
            {
                type = "object";
            }
            else
            {
                var systemTypeDic = ParameterParser.GetSystemTypeDic();
                foreach (var st in systemTypeDic)
                {
                    if (st.Key == type)
                    {
                        type = st.Value;
                        break;
                    }
                }
                if (ParameterParser.CheckIsArray(type))
                {
                    type = ParameterParser.GetArrayType(type);
                }
            }
            //switch (type)
            //{
            //    case "System.String":
            //        type = "string";
            //        format = "";
            //        break;
            //    case "System.Int32":
            //        type = "integer";
            //        format = "int32";
            //        break;
            //    case "System.Int64":
            //        type = "integer";
            //        format = "int64";
            //        break;
            //    case "System.Float":
            //        type = "number";
            //        format = "float";
            //        break;
            //    case "System.Double":
            //        type = "number";
            //        format = "double";
            //        break;
            //    case "System.Decimal":
            //        type = "number";
            //        format = "decimal";
            //        break;
            //    case "System.Byte":
            //        type = "string";
            //        format = "byte";
            //        break;
            //    case "System.Boolean":
            //        type = "boolean";
            //        format = "";
            //        break;
            //    case "System.DateTime":
            //        type = "datetime";
            //        format = "datetime";
            //        break;
            //    default:
            //        break;

            //}
            return (type, format);
        }
    }
}
