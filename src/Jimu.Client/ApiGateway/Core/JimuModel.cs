using Autofac;
using Jimu.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Jimu.Client.ApiGateway.Core
{
    [ModelBinder(BinderType = typeof(JimuModelBinder))]
    public class JimuModel
    {
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public JimuModel() { }
        public async void ReadFromContentAsync(Stream content, string contentType = "application/json")
        {

            using (var sr = new StreamReader(content))
            {
                var json = await sr.ReadToEndAsync();
                try
                {
                    Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                }
                catch (Exception ex)
                {
                    if (contentType == "text/plain" || contentType == "text/xml" || contentType == "application/xml")
                    {
                        Data = new Dictionary<string, object>();
                        Data.Add("data", json);
                    }
                    else
                    {
                        var logger = JimuClient.Host.Container.Resolve<ILogger>();
                        logger.Error($"JimuModel.ReadFromContentAsync, unsupport content-type: {contentType}, ", ex);
                    }

                }

            }
        }



        public JimuModel(IFormCollection form)
        {
            foreach (var f in form)
            {
                Data.Add(f.Key, f.Value);
            }
        }
        public JimuModel(Dictionary<string, object> data)
        {
            Data = data;
        }
    }
}
