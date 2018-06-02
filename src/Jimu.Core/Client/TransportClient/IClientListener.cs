using System;
using System.Threading.Tasks;

namespace Jimu.Client
{
    /// <summary>
    ///     listener for client recieve server response
    /// </summary>
    public interface IClientListener
    {
        /// <summary>
        ///     event of received server response
        /// </summary>
        event Func<IClientSender, JimuTransportMsg, Task> OnReceived;

        /// <summary>
        ///     how to handle server response
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task Received(IClientSender sender, JimuTransportMsg message);
    }
}