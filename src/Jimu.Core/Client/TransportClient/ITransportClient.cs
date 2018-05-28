using System.Threading.Tasks;
using Jimu.Core.Protocols;

namespace Jimu.Core.Client.TransportClient
{
    public interface ITransportClient
    {
        /// <summary>
        ///     send request
        /// </summary>
        /// <param name="invokeMessage"></param>
        /// <returns></returns>
        Task<RemoteInvokeResultMessage> SendAsync(RemoteInvokeMessage invokeMessage);

        //Task OnReceive(TransportMessage message);
    }
}