using System;
using System.Collections.Concurrent;
using System.Net;
using Jimu.Core.Client.TransportClient;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Protocols;

namespace Jimu.Core.Commons.Transport
{
    public class DefaultTransportClientFactory : ITransportClientFactory
    {
        private readonly ILogger _logger;

        public DefaultTransportClientFactory(ILogger logger)
        {
            Clients = new ConcurrentDictionary<EndPoint, Lazy<ITransportClient>>();
            _logger = logger;
        }

        public ConcurrentDictionary<EndPoint, Lazy<ITransportClient>> Clients { get; }


        public event CreatorDelegate ClientCreatorDelegate;

        public ITransportClient CreateClient<T>(T address)
            where T : Address
        {
            _logger.Info($"creating transport client for: {address.Code}");
            try
            {
                return Clients.GetOrAdd(address.CreateEndPoint(), ep => new Lazy<ITransportClient>(() =>
                {
                    ITransportClient client = null;
                    ClientCreatorDelegate?.Invoke(address, ref client);
                    _logger.Info($"succed to create transport client for: {address.Code}");
                    return client;
                })).Value;
            }
            catch (Exception ex)
            {
                Clients.TryRemove(address.CreateEndPoint(), out var value);
                _logger.Error($"failed to create transport client for : {address.Code}", ex);
                return null;
            }
        }
    }
}