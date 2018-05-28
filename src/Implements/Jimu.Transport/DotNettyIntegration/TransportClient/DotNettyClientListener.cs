using System;
using System.Threading.Tasks;
using Jimu.Core.Client.TransportClient;
using Jimu.Core.Protocols;

namespace Jimu.Common.Transport.DotNettyIntegration.TransportClient
{
    public class DotNettyClientListener : IClientListener
    {
        public event Func<IClientSender, TransportMessage, Task> OnReceived;
        public async Task Received(IClientSender sender, TransportMessage message)
        {
            if (OnReceived == null)
                return;
            await OnReceived(sender, message);
        }
    }
}
