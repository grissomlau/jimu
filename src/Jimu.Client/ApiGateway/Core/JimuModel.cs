using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Jimu.Client.ApiGateway
{
    [ModelBinder(BinderType = typeof(JimuModelBinder))]
    public class JimuModel
    {
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
        public JimuModel() { }
        public JimuModel(Stream content)
        {
            using (var sr = new StreamReader(content))
            {
                var json = sr.ReadToEnd();
                Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
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
