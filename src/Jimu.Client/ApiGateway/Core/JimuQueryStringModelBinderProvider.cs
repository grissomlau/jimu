using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jimu.Client.ApiGateway
{
    public class JimuQueryStringModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _modelBinder = new JimuQueryStringModelBinder(new SimpleTypeModelBinder(typeof(JimuQueryString),new NullLoggerFactory()));
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(JimuQueryString) ? _modelBinder : null;
        }
    }
}
