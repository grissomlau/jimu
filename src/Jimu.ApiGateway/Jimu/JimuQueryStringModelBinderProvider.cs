using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Jimu.ApiGateway
{
    public class JimuQueryStringModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _modelBinder = new JimuQueryStringModelBinder(new SimpleTypeModelBinder(typeof(JimuQueryString)));
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(JimuQueryString) ? _modelBinder : null;
        }
    }
}
