using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Jimu.Client.ApiGateway.Controllers
{
    //[Produces("application/json")]
    //[Route("api/[controller]")]
    public class JimuServicesController : Controller
    {
        [HttpGet, HttpPost]
        //public async Task<object> Path(string path, [FromQuery] MyQueryString query, [FromBody] Dictionary<string, object> model)
        public async Task<IActionResult> JimuPath(string path, [FromQuery] JimuQueryString query, [ModelBinder]JimuModel model)
        {
            var paras = new Dictionary<string, object>();
            if (model?.Data != null)
            {
                paras = model.Data;
            }
            if (query.Collection.Count > 0)
            {
                foreach (var key in query.Collection.AllKeys)
                {
                    paras[key] = query.Collection[key];
                }
            }
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            var result = await JimuClient.Invoke(path, paras);

            //if (result.ResultType != typeof(JimuFile).ToString())
            if (result?.ResultType != null && result.ResultType.StartsWith("{\"ReturnType\":\"Jimu.JimuFile\""))
            {
                var file = result.Result as JimuFile;
                return File(file?.Data, "application/octet-stream", file?.FileName);
            }
            return new JsonResult(result.Result);
        }
    }
}