using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jimu.Client.ApiGateway.Core
{
    public class JimuModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _modelBinder = new JimuModelBinder(new SimpleTypeModelBinder(typeof(JimuModel), new NullLoggerFactory()));
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.ModelType == typeof(JimuModel) ? _modelBinder : null;
        }
    }
}
