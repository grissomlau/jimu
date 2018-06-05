using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Jimu.ApiGateway.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Service")]
    public class ServicesController : Controller
    {
        [HttpGet, HttpPost]
        //public async Task<object> Path(string path, [FromQuery] MyQueryString query, [FromBody] Dictionary<string, object> model)
        public async Task<IActionResult> Path(string path, [FromQuery] JimuQueryString query, [ModelBinder]JimuModel model)
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
            var result = await JimuClient.Invoke(path, paras);

            if (result.ResultType != typeof(JimuFile).ToString())
                return new JsonResult(result.Result);

            var file = result.Result as JimuFile;
            return File(file?.Data, "application/octet-stream", file?.FileName);
        }
        //private string GetContentType(string path)
        //{
        //    var types = GetMimeTypes();
        //    var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
        //    return types[ext];
        //}

        //private Dictionary<string, string> GetMimeTypes()
        //{
        //    return new Dictionary<string, string>
        //    {
        //        {".txt", "text/plain"},
        //        {".pdf", "application/pdf"},
        //        {".doc", "application/vnd.ms-word"},
        //        {".docx", "application/vnd.ms-word"},
        //        {".xls", "application/vnd.ms-excel"},
        //        {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
        //        {".png", "image/png"},
        //        {".jpg", "image/jpeg"},
        //        {".jpeg", "image/jpeg"},
        //        {".gif", "image/gif"},
        //        {".csv", "text/csv"}
        //    };
        //}
    }
}