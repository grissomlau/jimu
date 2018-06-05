using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jimu.Client.LoadBalance.PollingIntegration
{
    /// <summary>
    ///     load balancing by polling algorithm
    /// </summary>
    public class PollingAddressSelector : AddressSelectorBase
    {
        private readonly ConcurrentDictionary<string, Lazy<ServerIndexHolder>> _addresses =
            new ConcurrentDictionary<string, Lazy<ServerIndexHolder>>();

        private readonly ILogger _logger;

        public PollingAddressSelector(ILogger logger)
        {
            _logger = logger;
        }

        public override Task<JimuAddress> GetAddressAsync(JimuServiceRoute serviceRoute)
        {
            var serverIndexHolder = _addresses.GetOrAdd(serviceRoute.ServiceDescriptor.Id,
                key => new Lazy<ServerIndexHolder>(() => new ServerIndexHolder()));
            var address = serverIndexHolder.Value.GetAddress(serviceRoute.Address.Where(x => x.IsHealth).ToList());
            _logger.Info($"ServerSelector: {serviceRoute.ServiceDescriptor.Id}: {address.Code}");
            return Task.FromResult(address);
        }

        private class ServerIndexHolder
        {
            private int _latestIndex;
            private int _lock;

            public JimuAddress GetAddress(List<JimuAddress> addresses)
            {
                while (true)
                {
                    if (Interlocked.Exchange(ref _lock, 1) != 0)
                    {
                        default(SpinWait).SpinOnce();
                        continue;
                    }

                    _latestIndex = (_latestIndex + 1) % addresses.Count;
                    var address = addresses[_latestIndex];
                    Interlocked.Exchange(ref _lock, 0);
                    return address;
                }
            }
        }
    }
}