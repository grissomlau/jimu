using System.Threading.Tasks;

namespace Jimu.Client.Transport
{
    public interface IClientSender
    {
        /// <summary>
        ///     send request
        /// </summary>
        /// <param name="invokeMessage"></param>
        /// <returns></returns>
        Task<JimuRemoteCallResultData> SendAsync(JimuRemoteCallData invokeMessage);

        /// <summary>
        /// check whether closed or disposed 
        /// </summary>
        /// <returns></returns>
        bool CheckValid();

        //Task OnReceive(TransportMessage message);
    }
}