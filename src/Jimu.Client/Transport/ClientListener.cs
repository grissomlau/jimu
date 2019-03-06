using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.Client
{
    /// <summary>
    ///     listener for client recieve server response
    /// </summary>
    public class ClientListener
    {
        /// <summary>
        ///     event of received server response
        /// </summary>
        public event Func<JimuTransportMsg, Task> OnReceived;

        /// <summary>
        ///     how to handle server response
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Received(JimuTransportMsg message)
        {
            if (OnReceived == null)
                return;
            await OnReceived( message);
        }
    }
}
