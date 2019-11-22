namespace Jimu.Client.ApiGateway.Swagger.Schema
{
    internal interface ISchema
    {
        SchemaModel GetProperties(JimuServiceParameterDesc para, ISchemaFactory schemaFactory);
    }
}
