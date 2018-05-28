using System;
using System.Collections.Concurrent;
using System.Net;
using Jimu.Core.Protocols;

namespace Jimu.Core.Client.TransportClient
{
    /// <summary>
    ///     delegate for how to create transport client
    /// </summary>
    /// <param name="add"></param>
    /// <param name="client"></param>
    public delegate void CreatorDelegate(Address add, ref ITransportClient client);

    public interface ITransportClientFactory
    {
        /// <summary>
        ///     all created client holded by the memory
        /// </summary>
        ConcurrentDictionary<EndPoint, Lazy<ITransportClient>> Clients { get; }

        /// <summary>
        ///     delegate for how to create transport client
        /// </summary>
        event CreatorDelegate ClientCreatorDelegate;

        ITransportClient CreateClient<T>(T address) where T : Address;
    }
}