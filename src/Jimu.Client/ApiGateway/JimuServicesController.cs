using Jimu.Common;
using Jimu.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace Jimu.Client.ApiGateway
{
    //[Produces("application/json")]
    //[Route("api/[controller]")]
    public class JimuServicesController : Controller
    {
        private ILogger _logger;
        private JimuOptions _jimuOptions;
        public JimuServicesController()
        {
            var loggerFactory = JimuClient.Host.Container.Resolve<ILoggerFactory>();
            _logger = loggerFactory.Create(this.GetType());
            _jimuOptions = JimuOptions.Get(JimuClient.Host.JimuAppSettings);
        }
        [HttpGet, HttpPost, HttpDelete, HttpPut, AllowAnonymous]
        //public async Task<object> Path(string path, [FromQuery] MyQueryString query, [FromBody] Dictionary<string, object> model)
        public async Task<IActionResult> JimuPath(string path, [FromQuery] JimuQueryString query)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            path = $"{path}{query.Query ?? ""}";
            // check max content length
            decimal contentLength = (decimal)(Request.ContentLength??0) / 1024 / 1024;
            if (contentLength > _jimuOptions.MaxRequestContentLengthMB)
            {
                var msg = $"content length({contentLength})M over the limit size of {_jimuOptions.MaxRequestContentLengthMB}M";
                _logger.Error(msg, null);
                return BadRequest(new JimuRemoteCallResultData
                {
                    ErrorCode = "400",
                    ErrorMsg = $"{path}, {msg}"
                });
            }
          
            // check wether service exists
            bool exists = await JimuClient.Exist(path, Request.Method);
            var paras = new Dictionary<string, object>();
            if (exists)
            {
                try
                {
                    paras = await ReadContentAsync(Request) ?? new Dictionary<string, object>();
                }
                catch (Exception ex)
                {
                    return BadRequest(new JimuRemoteCallResultData
                    {
                        ErrorCode = "400",
                        ErrorMsg = $"{path}, read body error. {ex.ToStackTraceString()}"
                    });
                }
                if (query.Collection.Count > 0)
                {
                    foreach (var key in query.Collection.Keys)
                    {
                        paras[key] = query.Collection[key];
                    }

                }
                exists = await JimuClient.Exist(path, Request.Method, paras);
            }
            if (!exists)
            {
                _logger.Debug($"{path} 404, service is null");
                return NotFound(new JimuRemoteCallResultData
                {
                    ErrorCode = "404",
                    ErrorMsg = $"{path}, not found!"
                });
            }

            // invoker
            var result = await JimuClient.Invoke(path, paras, Request.Method);

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

        private async Task<Dictionary<string, object>> ReadContentAsync(HttpRequest request)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            bool hasFormData = request.HasFormContentType && request.Form != null && (request.Form.Any() || request.Form.Files.Any());
            if (hasFormData)
            {
                var form = request.Form;
                if (form.Files.Any())
                {
                    var list = new List<JimuFile>();
                    foreach (var file in form.Files)
                    {
                        using (var sr = file.OpenReadStream())
                        {
                            var bytes = new byte[sr.Length];
                            await sr.ReadAsync(bytes, 0, bytes.Length);
                            var myFile = new JimuFile
                            {
                                FileName = file.FileName,
                                Data = bytes
                            };
                            list.Add(myFile);
                        }
                    }

                    return new Dictionary<string, object> { { "files", list } };

                }
                foreach (var f in form)
                {
                    data.Add(f.Key, f.Value);
                }
                return data;
            }
            // read data from body
            if (request.Body == null)
            {
                return data;
            }
            using (var sr = new StreamReader(Request.Body))
            {
                string json = null;

                try
                {
                    json = await sr.ReadToEndAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error($"JimuServicesController.ReadFromContentAsync, read body stream error.", ex);
                    throw;
                }
                try
                {
                    data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                }
                catch (Exception ex)
                {
                    string contentType = Request.ContentType;
                    if (contentType == "text/plain" || contentType == "text/xml" || contentType == "application/xml")
                    {
                        data = new Dictionary<string, object>();
                        data.Add("data", json);
                    }
                    else
                    {
                        _logger.Error($"JimuServicesController.ReadFromContentAsync, unsupport content-type: {contentType}, ", ex);
                        throw;
                    }
                }
                return data;
            }
        }

    }
}