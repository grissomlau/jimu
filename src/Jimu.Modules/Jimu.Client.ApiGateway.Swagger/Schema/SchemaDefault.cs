using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    class SchemaDefault : ISchema
    {
        public SchemaModel GetProperties(JimuServiceParameterDesc para, ISchemaFactory schemaFactory)
        {
            var model = new SchemaModel(para.Name, new OpenApiSchema
            {
                Type = para.Type,
                Title = para.Comment,
            });
            if (!string.IsNullOrEmpty(para.Default))
            {
                try
                {
                    switch (model.Value.Type)
                    {
                        case "string":
                            model.Value.Example = new OpenApiString(para.Default);
                            break;
                        case "integer":
                            model.Value.Example = new OpenApiInteger(Convert.ToInt32(para.Default));
                            break;
                        case "number":
                            model.Value.Example = new OpenApiDouble(Convert.ToDouble(para.Default));
                            break;
                        case "datetime":
                            model.Value.Example = new OpenApiDateTime(Convert.ToDateTime(para.Default));
                            break;
                        case "boolean":
                            model.Value.Example = new OpenApiBoolean(Convert.ToBoolean(para.Default));
                            break;
                        default:
                            break;

                    }
                }
                catch { }
            }

            return model;
        }
    }
}
