using System;
using Newtonsoft.Json;

namespace Jimu.Core.Protocols
{
    /// <summary>
    ///     wrap the remoteInvokeMessage or remoteInvokeResultMessage when transfer between client and server
    /// </summary>
    public class TransportMessage
    {
        public TransportMessage(object content)
        {
            Content = content;
            ContentType = content.GetType().ToString();
        }

        public string Id { get; set; }
        public string ContentType { get; set; }
        public object Content { get; set; }

        public bool IsInvokeMessage()
        {
            return ContentType == typeof(RemoteInvokeMessage).FullName;
        }

        public bool IsInvokeResultMessage()
        {
            return ContentType == typeof(RemoteInvokeResultMessage).FullName;
        }

        public T GetContent<T>()
        {
            try
            {
                return (T)Content;
            }
            catch (Exception)
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(Content));
            }
        }

        public static TransportMessage Create(RemoteInvokeMessage invokeMessage)
        {
            return new TransportMessage(invokeMessage)
            {
                Id = Guid.NewGuid().ToString("N")
            };
        }


        public static TransportMessage Create(string id, RemoteInvokeResultMessage invokeResultMessage)
        {
            return new TransportMessage(invokeResultMessage)
            {
                Id = id
            };
        }
    }
}