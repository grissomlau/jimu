using System.Threading.Tasks;
using Jimu.Core.Protocols;

namespace Jimu.Core.Client.TransportClient
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
        Task SendAsync(TransportMessage msg);
    }
}