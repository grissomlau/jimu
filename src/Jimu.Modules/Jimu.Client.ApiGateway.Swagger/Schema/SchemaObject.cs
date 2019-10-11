using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    class SchemaObject : ISchema
    {
        public SchemaModel GetProperties(JimuServiceParameterDesc para, ISchemaFactory schemaFactory)
        {
            return new SchemaModel(para.Name, new OpenApiSchema
            {
                Type = "object",
                Title = para.Comment,
                Properties = schemaFactory.GetProperties(para.Properties)
            });
        }
    }
}
