using Autofac;
using Jimu.Client.ApiGateway.Core;
using Jimu.Client.ApiGateway.Swagger.Schema;
using Jimu.Client.Discovery;
using Jimu.Common;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Jimu.Client.ApiGateway.Swagger
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {

        static ISchemaFactory _schemaFactory = new SchemaFactory();
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var serviceDiscovery = JimuClient.Host.Container.Resolve<IClientServiceDiscovery>();
            var routes = serviceDiscovery.GetRoutesAsync().GetAwaiter().GetResult();
            var groupRoutes = routes.GroupBy(x =>
            {
                if (x.ServiceDescriptor.RoutePath.Contains('?'))
                {
                    return x.ServiceDescriptor.RoutePath.Split('?')[0];
                }
                return x.ServiceDescriptor.RoutePath;
            });

            swaggerDoc.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme> {
                { "bearerAuth",new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "bearerAuth",
                    Reference = new OpenApiReference()
                    {
                        Id = "bearerAuth",
                        Type = ReferenceType.SecurityScheme
                    },
                    UnresolvedReference = false
                } }

            };

            foreach (var gr in groupRoutes)
            {
                var pureRoute = gr.Key;
                if (gr.Key.Contains('?'))
                {
                    pureRoute = gr.Key.Split('?')[0];
                }
                var pathItem = new OpenApiPathItem();

                foreach (var r in gr)
                {
                    var x = r.ServiceDescriptor;
                    var paras = new List<OpenApiParameter>();
                    var jimuParas = new List<JimuServiceParameterDesc>();
                    if (!string.IsNullOrEmpty(x.Parameters))
                    {
                        jimuParas = JimuHelper.Deserialize(TypeHelper.ReplaceTypeToJsType(x.Parameters), typeof(List<JimuServiceParameterDesc>)) as List<JimuServiceParameterDesc>;
                        paras = GetParameters(r.ServiceDescriptor.RoutePath, jimuParas, x.HttpMethod);
                    }
                    var responses = new OpenApiResponses();
                    responses.Add("200", GetResponse(x.ReturnDesc));


                    OpenApiOperation operation = new OpenApiOperation
                    {
                        OperationId = x.RoutePath,
                        Parameters = paras,
                        Responses = responses,
                        Description = x.Comment,
                        Summary = x.Comment,
                        Tags = GetTags(x),
                        RequestBody = GetRequestBody(r.ServiceDescriptor.RoutePath, jimuParas, x.HttpMethod)
                    };
                    if (Enum.TryParse(typeof(OperationType), CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.HttpMethod.ToLower()), out var opType))
                    {
                        pathItem.AddOperation((OperationType)opType, operation);
                    }
                    if (!x.GetMetadata<bool>("AllowAnonymous"))
                    {
                        operation.Security = new List<OpenApiSecurityRequirement> {
                           new OpenApiSecurityRequirement{
                            {
                                new OpenApiSecurityScheme {
                                    Reference = new OpenApiReference()
                                            {
                                                Id = "bearerAuth",
                                                Type = ReferenceType.SecurityScheme
                                            },
                                    UnresolvedReference = true   },
                                new List<string>() }
                           }
                        };
                    }
                }

                swaggerDoc.Paths.Add(pureRoute, pathItem);
            }
        }

        private List<OpenApiTag> GetTags(JimuServiceDesc desc)
        {
            string tag = desc.Service;
            if (!string.IsNullOrEmpty(desc.ServiceComment))
            {
                tag += $":{desc.ServiceComment}";
            }
            if (!string.IsNullOrEmpty(tag))
            {
                return new List<OpenApiTag>
                {
                    new OpenApiTag
                    {
                         Description = tag,
                          Name = tag
                    }
                };

            }
            return null;

        }

        private static OpenApiResponse GetResponse(string returnDescStr)
        {

            if (string.IsNullOrEmpty(returnDescStr) || !returnDescStr.StartsWith('{'))
            {
                var resp = new OpenApiResponse
                {
                    Description = "Success",
                    Content = new Dictionary<string, OpenApiMediaType>()
                };
                resp.Content.Add("application/json", new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Default = new OpenApiString(returnDescStr)
                    }
                });
                return resp;
            }
            var returnDesc = JsonConvert.DeserializeObject<JimuServiceReturnDesc>(TypeHelper.ReplaceTypeToJsType(returnDescStr));
            var response = new OpenApiResponse
            {
                Description = string.IsNullOrEmpty(returnDesc.Comment) ? "Success" : returnDesc.Comment,
                Content = new Dictionary<string, OpenApiMediaType>()
            };

            var isVoid = returnDesc.ReturnType == "System.Void";
            if (isVoid)
            {
                return response;
            }

            var isObject = TypeHelper.CheckIsObject(returnDesc.ReturnType);
            var isArray = TypeHelper.CheckIsArray(returnDesc.ReturnType);
            var schema = new OpenApiSchema
            {
                Type = returnDesc.ReturnType
            };

            if ((isObject || isArray) && returnDesc.Properties != null && returnDesc.Properties.Any())
            {
                if (isArray)
                {
                    schema = new OpenApiSchema
                    {
                        Type = "array",
                        Title = returnDesc.Comment,
                        Description = returnDesc.Comment,
                        Items = new OpenApiSchema { Properties = _schemaFactory.GetProperties(returnDesc.Properties) }
                    };
                }
                else
                {
                    schema = new OpenApiSchema
                    {
                        Type = "object",
                        Title = returnDesc.Comment,
                        Description = returnDesc.Comment,
                        Properties = _schemaFactory.GetProperties(returnDesc.Properties)
                    };
                }
            }
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = schema
            });
            return response;
        }

        private static OpenApiRequestBody GetRequestBody(string route, List<JimuServiceParameterDesc> paras, string httpMethod)
        {
            if (httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            var bodyParas = paras.Where(x => route.IndexOf($"{{{x.Name}}}") < 0).ToList();

            var reqBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>(),
            };
            var schema = new OpenApiSchema
            {
                Type = "object",
                Properties = _schemaFactory.GetProperties(bodyParas)
            };
            reqBody.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = schema
            });
            return reqBody;
        }

        private static List<OpenApiParameter> GetParameters(string route, List<JimuServiceParameterDesc> paras, string httpMethod)
        {
            List<OpenApiParameter> parameters = new List<OpenApiParameter>();
            var pureRoute = route;
            var query = "";
            if (route.Contains("?"))
            {
                var arr = route.Split('?');
                pureRoute = arr[0];
                query = arr[1];
            }

            foreach (var p in paras)
            {
                OpenApiParameter param = null;
                if (pureRoute.IndexOf($"{{{p.Name}}}") > 0)
                {
                    param = new OpenApiParameter
                    {
                        Name = p.Name,
                        In = ParameterLocation.Path,
                        Description = $"{p.Comment}",
                        Schema = new OpenApiSchema
                        {
                            Type = p.Type,
                            Properties = _schemaFactory.GetProperties(p.Properties)
                        }
                    };

                }
                else if (query.IndexOf($"{{{p.Name}}}") > 0 && httpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    param = new OpenApiParameter
                    {
                        Name = p.Name,
                        In = ParameterLocation.Query,
                        Description = $"{p.Comment}",
                        Schema = new OpenApiSchema
                        {
                            Type = p.Type,
                            Properties = _schemaFactory.GetProperties(p.Properties)
                        }
                    };
                }
                if (param != null)
                {
                    if (TypeHelper.CheckIsArray(p.Type))
                    {
                        param.Schema.Type = "array";
                        param.Schema.Items = new OpenApiSchema
                        {
                            Properties = param.Schema.Properties
                        };
                        if (param.In == ParameterLocation.Query)
                        {
                            param.Explode = true;
                        }
                    }
                    else if (TypeHelper.CheckIsObject(p.Type))
                    {
                        param.Schema.Type = "object";
                        param.Style = ParameterStyle.Form;
                        param.Explode = false;
                    }
                    parameters.Add(param);
                }
            }
            return parameters;
        }
    }


}
