using System.Threading.Tasks;

namespace Jimu.Client
{
    /// <summary>
    ///     sender for client
    /// </summary>
    public interface IClientSender
    {
        /// <summary>
        ///     send msg to server
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        Task SendAsync(JimuTransportMsg msg);
    }
}