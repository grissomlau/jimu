using Microsoft.OpenApi.Models;

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
