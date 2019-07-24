using Jimu.Logger;
using System;
using System.Collections.Concurrent;
using System.Net;

namespace Jimu.Client
{
    public class ClientSenderFactory
    {
        /// <summary>
        ///     delegate for how to create transport client
        /// </summary>
        /// <param name="add"></param>
        /// <param name="client"></param>
        public delegate void ClientSenderCreatorDelegate(JimuAddress add, ref IClientSender client);

        private readonly ILogger _logger;

        //public ConcurrentDictionary<EndPoint, Lazy<ITransportClient>> Clients { get; }
        /// <summary>
        ///     all created client holded by the memory
        /// </summary>
        public ConcurrentDictionary<string, Lazy<IClientSender>> ClientSenders { get; }

        /// <summary>
        ///     delegate for how to create transport client
        /// </summary>
        public event ClientSenderCreatorDelegate ClientSenderCreator;

        public ClientSenderFactory(ILogger logger)
        {
            //Clients = new ConcurrentDictionary<EndPoint, Lazy<ITransportClient>>();
            ClientSenders = new ConcurrentDictionary<string, Lazy<IClientSender>>();
            _logger = logger;
        }

        public IClientSender CreateClientSender<T>(T address)
            where T : JimuAddress
        {
            try
            {
                _logger.Debug($"creating transport client for: {address.Code}");
                //return Clients.GetOrAdd(address.CreateEndPoint(), ep => new Lazy<ITransportClient>(() =>
                var val = ClientSenders.GetOrAdd($"{address.Protocol}-{address.Code}", (ep => new Lazy<IClientSender>((() =>
               {
                   IClientSender client = null;
                   ClientSenderCreator?.Invoke(address, ref client);
                   return client;
               })))).Value;
                if (!val.CheckValid())
                {
                    _logger.Debug($"transport client for: {address.Code} is invalid, retry to created");
                    ClientSenders.TryRemove($"{address.Protocol}-{address.Code}", out _);
                    val = ClientSenders.GetOrAdd($"{address.Protocol}-{address.Code}", (ep => new Lazy<IClientSender>((() =>
                                  {
                                      IClientSender client = null;
                                      ClientSenderCreator?.Invoke(address, ref client);
                                      return client;
                                  })))).Value;


                }
                _logger.Debug($"succed to create transport client for: {address.Code}");
                return val;
            }
            catch (Exception ex)
            {
                //Clients.TryRemove(address.CreateEndPoint(), out _);
                ClientSenders.TryRemove($"{address.Protocol}-{address.Code}", out _);
                _logger.Error($"failed to create transport client for : {address.Code}", ex);
                throw new TransportException(ex.Message, ex);
            }
        }
    }
}