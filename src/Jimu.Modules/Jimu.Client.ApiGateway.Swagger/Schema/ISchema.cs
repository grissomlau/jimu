using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    internal interface ISchema
    {
        SchemaModel GetProperties(JimuServiceParameterDesc para, ISchemaFactory schemaFactory);
    }
}
