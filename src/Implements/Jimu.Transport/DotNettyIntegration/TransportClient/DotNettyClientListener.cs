using System;
using System.Threading.Tasks;

namespace Jimu.Client
{
    public class DotNettyClientListener : IClientListener
    {
        public event Func<IClientSender, JimuTransportMsg, Task> OnReceived;
        public async Task Received(IClientSender sender, JimuTransportMsg message)
        {
            if (OnReceived == null)
                return;
            await OnReceived(sender, message);
        }
    }
}
