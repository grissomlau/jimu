using Microsoft.OpenApi.Models;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    class SchemaDateTime : ISchema
    {
        public SchemaModel GetProperties(JimuServiceParameterDesc para, ISchemaFactory schemaFactory)
        {
            return new SchemaModel(para.Name, new OpenApiSchema
            {
                Type = "string",
                Format = "date-time",
                Title = para.Comment
            });
        }
    }
}
