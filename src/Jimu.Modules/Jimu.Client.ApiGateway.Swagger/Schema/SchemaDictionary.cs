using Microsoft.OpenApi.Models;
using System.Linq;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    class SchemaDictionary : ISchema
    {
        public SchemaModel GetProperties(JimuServiceParameterDesc para, ISchemaFactory schemaFactory)
        {
            var schema = new OpenApiSchema
            {
                Type = "array",
                Title = para.Comment,
            };
            var props = schemaFactory.GetProperties(para.Properties);
            var dicSchema = new OpenApiSchema();
            if (props["value"].Properties != null && props["value"].Properties.Any())
            {
                dicSchema.Properties.Add(props["key"].Type,
                    new OpenApiSchema
                    {
                        Properties = props["value"].Properties
                    });
            }
            else
            {
                dicSchema.Properties.Add(props["key"].Type, props["value"]);
            }
            schema.Items = dicSchema;
            return new SchemaModel(para.Name, schema);
        }
    }
}
