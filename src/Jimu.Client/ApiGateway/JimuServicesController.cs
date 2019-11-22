using Jimu.Client.ApiGateway.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client.ApiGateway
{
    //[Produces("application/json")]
    //[Route("api/[controller]")]
    public class JimuServicesController : Controller
    {
        [HttpGet, HttpPost, HttpDelete, HttpPut, AllowAnonymous]
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
            var result = await JimuClient.Invoke($"{path}{(query.Query ?? "")}", paras, Request.Method);

            if (result?.ResultType != null && result.ResultType.StartsWith("{\"ReturnType\":\"Jimu.JimuRedirect\""))
            {
                var redirect = result.Result as JimuRedirect;
                return Redirect(redirect.RedirectUrl);
            }
            //if (result.ResultType != typeof(JimuFile).ToString())
            if (result?.ResultType != null && result.ResultType.StartsWith("{\"ReturnType\":\"Jimu.JimuFile\""))
            {
                var file = result.Result as JimuFile;
                return File(file?.Data, "application/octet-stream", file?.FileName);
            }
            //return new JsonResult(result.Result);
            return Ok(result.Result);
        }
    }
}