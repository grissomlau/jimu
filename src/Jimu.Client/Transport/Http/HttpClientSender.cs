using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Jimu.Client
{
    public class HttpClientSender : IClientSender
    {
        readonly JimuAddress _address;
        readonly IClientListener _clientListener;
        public HttpClientSender(JimuAddress address, IClientListener clientListener)
        {
            _address = address;
            _clientListener = clientListener;
        }
        public async Task SendAsync(JimuTransportMsg msg)
        {
            var invokeMessage = msg.GetContent<JimuRemoteCallData>();
            var uri = $"http://{_address.CreateEndPoint()}/{invokeMessage.Descriptor.RoutePath}";
            //if (invokeMessage.Descriptor.HttpMethod() == "get" || invokeMessage.Descriptor.HttpMethod() == null)
            //{
            foreach (var para in invokeMessage.Parameters)
            {
                uri.SetQueryParam(para.Key, para.Value);
            }
            //var ret = await uri.SendJsonAsync(HttpMethod.Get, msg);
            //var str = await ret.Content.ReadAsStringAsync();
            var result = await uri.SendJsonAsync(HttpMethod.Get, msg).ReceiveJson<JimuTransportMsg>();
            //result = await uri.GetJsonAsync<RemoteInvokeResultMessage>();

            //return new RemoteInvokeResultMessage { Result = result, ResultType = result.GetType().ToString() };
            //}
            //else if (invokeMessage.Descriptor.HttpMethod() == "post")
            //{
            //    switch (invokeMessage.Descriptor.HttpContentType())
            //    {
            //        case "application/json":
            //        case null:
            //            result = await uri.PostJsonAsync(invokeMessage.Parameters).ReceiveJson<TransportMessage>();
            //            break;
            //        case "application/x-www-form-urlencoded":
            //            result = await uri.PostUrlEncodedAsync(invokeMessage.Parameters).ReceiveJson<TransportMessage>();
            //            break;
            //        case "text/plain":
            //            StringBuilder paraSb = new StringBuilder();
            //            foreach (var para in invokeMessage.Parameters)
            //            {
            //                paraSb.Append($"{para.Key}={para.Value}&");
            //            }
            //            result = await uri.PostStringAsync(paraSb.Length > 0 ? paraSb.ToString().TrimEnd('&') : "").ReceiveJson<TransportMessage>();
            //            break;
            //        case "multipart/form-data":
            //            var ret = await uri.PostMultipartAsync((bc) =>
            //            {
            //                foreach (var para in invokeMessage.Parameters)
            //                {
            //                    if (para.Value.GetType().IsAssignableFrom(typeof(HttpContent)))
            //                    {
            //                        bc.Add((HttpContent)para.Value);
            //                    }
            //                    //bc.Add(para.Key, (HttpContent)para.Value);
            //                }
            //            });
            //            result = TransportMessage.Create(msg.Id, new RemoteInvokeResultMessage { Result = ret });
            //            break;
            //        default:
            //            result = TransportMessage.Create(msg.Id, new RemoteInvokeResultMessage { ErrorCode = "501", ErrorMsg = "not support media type" });
            //            break;
            //    }
            //}
            //result = new RemoteInvokeResultMessage { ErrorCode = "501", ErrorMsg = "not support media type" };
            await _clientListener.Received(this, result);
        }
    }
}
