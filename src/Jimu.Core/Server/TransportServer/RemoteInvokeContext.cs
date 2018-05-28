using System;
using System.Linq;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Protocols;
using Jimu.Core.Server.ServiceContainer;

namespace Jimu.Core.Server.TransportServer
{
    /// <summary>
    ///     context of service invoker
    /// </summary>
    public class RemoteInvokeContext
    {
        public RemoteInvokeContext(TransportMessage transportMessage, IServiceEntryContainer serviceEntryContainer,
            IResponse response, ILogger logger)
        {
            Response = response;
            TransportMessage = transportMessage;
            try
            {
                RemoteInvokeMessage = transportMessage.GetContent<RemoteInvokeMessage>();
            }
            catch (Exception ex)
            {
                logger.Error("failed to convert transportmessage.content to  RemoteInvokeMessage.", ex);
                return;
            }

            ServiceEntry = serviceEntryContainer.GetServiceEntry()
                .FirstOrDefault(x => x.Descriptor.Id == RemoteInvokeMessage.ServiceId);
            if (ServiceEntry == null)
            {
                logger.Info($"not found service: {RemoteInvokeMessage.ServiceId}");
            }
        }

        public ServiceEntry ServiceEntry { get; }
        public IResponse Response { get; }

        public TransportMessage TransportMessage { get; }

        public RemoteInvokeMessage RemoteInvokeMessage { get; }
    }
}