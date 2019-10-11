using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    class SchemaArray : ISchema
    {
        public SchemaModel GetProperties(JimuServiceParameterDesc para, ISchemaFactory schemaFactory)
        {
            var schema = new OpenApiSchema
            {
                Type = "array",
                Title = para.Comment
            };
            if (para.Properties != null && para.Properties.Count == 1 && (para.Properties.First().Properties == null || !para.Properties.First().Properties.Any()))
            {
                schema.Items = new OpenApiSchema
                {
                    Type = para.Properties.First().Type
                };

            }
            else
            {
                schema.Items = new OpenApiSchema
                {
                    Properties = schemaFactory.GetProperties(para.Properties)
                };
            }
            return new SchemaModel(para.Name, schema);
        }
    }
}
