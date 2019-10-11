using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.Models;

namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    class SchemaFactory : ISchemaFactory
    {
        public Dictionary<string, OpenApiSchema> GetProperties(List<JimuServiceParameterDesc> paras)
        {
            Dictionary<string, OpenApiSchema> pros = new Dictionary<string, OpenApiSchema>();
            if (paras == null) return pros;

            foreach (var para in paras)
            {
                SchemaModel model = null;
                var isNullType = para.Type == null;
                if (!isNullType && para.Type.StartsWith("System.Collections.Generic.Dictionary"))
                {
                    model = new SchemaDictionary().GetProperties(para, this);
                }
                else if (!isNullType &&
                    (para.Type.EndsWith("[]") || para.Type.StartsWith("System.Collections.Generic")))
                {
                    model = new SchemaArray().GetProperties(para, this);
                }
                else if (!isNullType &&
                    (para.Type == "datetime" || para.Type == "date"))
                {
                    model = new SchemaDateTime().GetProperties(para, this);
                }
                else if (para.Properties != null && para.Properties.Any())
                {
                    model = new SchemaObject().GetProperties(para, this);
                }
                else
                {
                    model = new SchemaDefault().GetProperties(para, this);
                }

                if (model != null)
                    pros.Add(model.Key, model.Value);
            }
            return pros;
        }
    }
}
