using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Jimu.Client.ApiGateway
{
    public class JimuQueryStringModelBinder : IModelBinder
    {
        private readonly IModelBinder _modelBinder;
        public JimuQueryStringModelBinder(IModelBinder modelBinder)
        {
            _modelBinder = modelBinder;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var query = bindingContext.ActionContext.HttpContext.Request.QueryString.Value;
            var queryString = new JimuQueryString(query);
            bindingContext.ModelState.SetModelValue("query", queryString, null);
            bindingContext.Result = ModelBindingResult.Success(queryString);
            return Task.CompletedTask;
        }
    }
}
