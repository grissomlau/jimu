using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.SwaggerIntegration
{
    class AuthorizationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            //var filtetPipeline = apiDescription.ActionDescriptor.GetFilterPipeline();
            //context.
            //var isAuthorized =
            //    filtetPipeline.Select(filterInfo => filterInfo.Instance).Any(filter => filter is IAuthorizationFilter);
        }
    }
}
