using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    public class SchemaModel
    {
        public SchemaModel(string key, OpenApiSchema value)
        {
            this.Key = key;
            this.Value = value;
        }
        public string Key { get; set; }
        public OpenApiSchema Value { get; set; }
    }
}
