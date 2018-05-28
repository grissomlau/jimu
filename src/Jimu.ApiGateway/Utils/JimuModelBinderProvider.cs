using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Jimu.ApiGateway.Utils
{
    public class JimuModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _modelBinder = new JimuModelBinder(new SimpleTypeModelBinder(typeof(JimuModel)));
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(JimuModel) ? _modelBinder : null;
        }
    }
}
