using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    internal interface ISchemaFactory
    {
        Dictionary<string, OpenApiSchema> GetProperties(List<JimuServiceParameterDesc> paras);
    }
}
