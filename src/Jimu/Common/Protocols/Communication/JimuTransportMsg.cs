using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jimu
{
    /// <summary>
    ///     wrap the remoteInvokeMessage or remoteInvokeResultMessage when transfer between client and server
    /// </summary>
    public class JimuTransportMsg
    {
        public JimuTransportMsg() { }
        public JimuTransportMsg(object content)
        {
            Id = Guid.NewGuid().ToString("N");
            Content = content;
            ContentType = content.GetType().ToString();
        }
        public JimuTransportMsg(string id, object content)
        {
            Id = id;
            Content = content;
            ContentType = content.GetType().ToString();
        }

        public string Id { get; set; }
        public string ContentType { get; set; }
        public object Content { get; set; }

        public T GetContent<T>()
        {
            try
            {
                if (Content is JObject o)
                {
                    return o.ToObject<T>();
                }
                return (T)Content;
            }
            catch (Exception)
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(Content));
            }
        }
    }
}