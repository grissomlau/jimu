using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Updraft.ApiGateway.Utils
{
    public class MyQueryStringValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            context.ValueProviders.Insert(0, new MyQueryStringValueProvider(context.ActionContext.HttpContext.Request.Query));
            return Task.CompletedTask;
        }
    }
}
