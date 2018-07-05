using System;
using System.Collections.Concurrent;
using System.Net;

namespace Jimu.Client
{
    public class DefaultTransportClientFactory : ITransportClientFactory
    {
        private readonly ILogger _logger;

        public DefaultTransportClientFactory(ILogger logger)
        {
            //Clients = new ConcurrentDictionary<EndPoint, Lazy<ITransportClient>>();
            Clients = new ConcurrentDictionary<string, Lazy<ITransportClient>>();
            _logger = logger;
        }

        //public ConcurrentDictionary<EndPoint, Lazy<ITransportClient>> Clients { get; }
        public ConcurrentDictionary<string, Lazy<ITransportClient>> Clients { get; }


        public event CreatorDelegate ClientCreatorDelegate;

        public ITransportClient CreateClient<T>(T address)
            where T : JimuAddress
        {
            _logger.Info($"creating transport client for: {address.Code}");
            try
            {
                //return Clients.GetOrAdd(address.CreateEndPoint(), ep => new Lazy<ITransportClient>(() =>
                return Clients.GetOrAdd($"{address.ServerFlag}-{address.Code}", ep => new Lazy<ITransportClient>(() =>
                {
                    ITransportClient client = null;
                    ClientCreatorDelegate?.Invoke(address, ref client);
                    _logger.Info($"succed to create transport client for: {address.Code}");
                    return client;
                })).Value;
            }
            catch (Exception ex)
            {
                //Clients.TryRemove(address.CreateEndPoint(), out _);
                Clients.TryRemove($"{address.ServerFlag}-{address.Code}", out _);
                _logger.Error($"failed to create transport client for : {address.Code}", ex);
                throw new TransportException(ex.Message, ex);
            }
        }
    }
}