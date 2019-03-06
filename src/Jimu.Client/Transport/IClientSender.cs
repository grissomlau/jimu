using System.Threading.Tasks;

namespace Jimu.Client
{
    public interface IClientSender
    {
        /// <summary>
        ///     send request
        /// </summary>
        /// <param name="invokeMessage"></param>
        /// <returns></returns>
        Task<JimuRemoteCallResultData> SendAsync(JimuRemoteCallData invokeMessage);

        //Task OnReceive(TransportMessage message);
    }
}