using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    internal interface ISchemaFactory
    {
        Dictionary<string, OpenApiSchema> GetProperties(List<JimuServiceParameterDesc> paras);
    }
}
