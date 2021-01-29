using Consul;
using Jimu.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jimu.Server.Discovery.Consul
{
    public class ConsulServiceDiscovery : IServiceDiscovery, IDisposable
    {
        private readonly ConsulClient _consul;
        private readonly List<string> _serviceGroups;
        private readonly string _serverAddress;

        private List<JimuServiceRoute> _routes;

        public event Action<List<JimuServiceRoute>> OnBeforeSetRoutes;

        public ConsulServiceDiscovery(string ip, int port, string serviceGroups, string serverAddress)
        {
            _routes = new List<JimuServiceRoute>();
            _serviceGroups = serviceGroups.Split(',').ToList();
            _serverAddress = serverAddress;
            _consul = new ConsulClient(config =>
            {
                config.Address = new Uri($"http://{ip}:{port}");
            });
        }

        public void Dispose()
        {
            _consul.Dispose();
        }

        public async Task<List<JimuServiceRoute>> GetRoutesAsync()
        {
            if (_routes != null && _routes.Any())
            {
                return _routes.ToList();
            }
            foreach (var keyPattern in this.GetKey(""))
            {
                var queryResult = await _consul.KV.List(keyPattern);
                var response = queryResult.Response;
                if (response == null)
                {
                    continue;
                }
                foreach (var key in response)
                {
                    if (key.Value == null)
                    {
                        continue;
                    }

                    var descriptors = JimuHelper.Deserialize<byte[], List<JimuServiceRouteDesc>>(key.Value);
                    if (descriptors != null && descriptors.Any())
                    {
                        foreach (var descriptor in descriptors)
                        {
                            if (_routes.Any(x => x.ServiceDescriptor.Id == descriptor.ServiceDescriptor.Id))
                            {
                                continue;
                            }
                            List<JimuAddress> addresses = new List<JimuAddress>(descriptor.AddressDescriptors.ToArray().Count());
                            foreach (var addDesc in descriptor.AddressDescriptors)
                            {
                                //var addrType = Type.GetType(addDesc.Type);
                                addresses.Add(JimuHelper.Deserialize(addDesc.Value, typeof(JimuAddress)) as JimuAddress);
                            }

                            _routes.Add(new JimuServiceRoute
                            {
                                Address = addresses,
                                ServiceDescriptor = descriptor.ServiceDescriptor
                            });
                        }
                    }
                }
            }


            return _routes;
        }

        public async Task<List<JimuAddress>> GetAddressAsync()
        {
            var routes = await GetRoutesAsync();
            var addresses = new List<JimuAddress>();
            if (routes != null && routes.Any())
            {
                addresses = routes.SelectMany(x => x.Address).Distinct().ToList();
            }
            return addresses;
        }

        public async Task ClearAsync()
        {
            //foreach (var key in GetKey())
            //{
            //    await _consul.KV.Delete(key);
            //}
            foreach (var key in this.GetKey(""))
            {
                var queryResult = await _consul.KV.List(key);
                var response = queryResult.Response;
                if (response != null)
                {
                    foreach (var result in response)
                    {
                        await _consul.KV.DeleteCAS(result);
                    }
                }
            }
        }

        public async Task SetRoutesAsync(List<JimuServiceRoute> routes)
        {
            OnBeforeSetRoutes?.Invoke(routes);
            //var existingRoutes = await GetRoutes(routes.Select(x => GetKey(x.ServiceDescriptor.Id)));
            _routes = routes.ToList();
            var routeDescriptors = new List<JimuServiceRouteDesc>(routes.Count());
            foreach (var route in routes)
            {
                routeDescriptors.Add(new JimuServiceRouteDesc
                {
                    ServiceDescriptor = route.ServiceDescriptor,
                    AddressDescriptors = route.Address?.Select(x => new JimuAddressDesc
                    {
                        //Type = $"{x.GetType().FullName}, {x.GetType().Assembly.GetName()}",
                        Value = JimuHelper.Serialize<string>(x)
                    })
                });
            }

            //await SetRoutesAsync(routeDescriptors);
            var group = routeDescriptors.GroupBy(x => x.ServiceDescriptor.ServiceClassPath);
            foreach (var classPath in group)
            {
                var routeDescList = classPath.ToList();
                var nodeData = JimuHelper.Serialize<byte[]>(routeDescList);
                foreach (var key in GetKey(classPath.Key))
                {
                    var keyValuePair = new KVPair(key) { Value = nodeData };
                    await _consul.KV.Put(keyValuePair);
                }
            }
        }

        //public async Task AddRouteAsync(List<JimuServiceRoute> routes)
        //{
        //    var curRoutes = await GetRoutesAsync();
        //    foreach (var route in routes)
        //    {
        //        curRoutes.RemoveAll(x => x.ServiceDescriptor.Id == route.ServiceDescriptor.Id);
        //        curRoutes.Add(route);
        //    }
        //    await SetRoutesAsync(curRoutes);
        //}

        private List<string> GetKey(string key)
        {
            return _serviceGroups.Select(x => $"{x}-{_serverAddress}-{key}").ToList();
        }

        private async Task<JimuServiceRoute> GetRoute(string path)
        {
            var queryResult = await _consul.KV.Keys(path);
            if (queryResult.Response == null)
            {
                return null;
            }
            var data = (await _consul.KV.Get(path)).Response?.Value;
            if (data == null)
            {
                return null;
            }
            var descriptor = JimuHelper.Deserialize<byte[], JimuServiceRouteDesc>(data);

            List<JimuAddress> addresses = new List<JimuAddress>(descriptor.AddressDescriptors.ToArray().Count());
            foreach (var addDesc in descriptor.AddressDescriptors)
            {
                //var addrType = Type.GetType(addDesc.Type);
                addresses.Add(JimuHelper.Deserialize(addDesc.Value, typeof(JimuAddress)) as JimuAddress);
            }

            return new JimuServiceRoute
            {
                Address = addresses,
                ServiceDescriptor = descriptor.ServiceDescriptor
            };
        }

        ///// <summary>
        ///// clean specify address service
        ///// </summary>
        ///// <param name="address"></param>
        ///// <returns></returns>
        //public async Task ClearAsync(string address)
        //{
        //    await _consul.KV.Delete(GetKey());
        //    //var routes = await GetRoutesAsync();
        //    //var remoteRouteIds = (from route in routes
        //    //                      where route.Address.Count() == 1 && (route.Address.Any(x => x == null || route.Address.First().Code == address))
        //    //                      select route.ServiceDescriptor.Id).ToArray();
        //    //foreach (var remoteRouteId in remoteRouteIds)
        //    //{
        //    //    await _consul.KV.Delete(GetKey(remoteRouteId));
        //    //}
        //}

        public async Task ClearServiceAsync(string serviceId)
        {
            var routes = await GetRoutesAsync();
            var hasRemove = routes.RemoveAll(x => x.ServiceDescriptor.Id == serviceId);
            if (hasRemove > 0)
                await SetRoutesAsync(routes);
        }

        public async Task<List<JimuServiceRoute>> RefreshRoutesAsync()
        {
            this._routes.Clear();
            foreach (var keyPattern in this.GetKey(""))
            {
                var queryResult = await _consul.KV.List(keyPattern);
                var response = queryResult.Response;
                if (response == null)
                {
                    continue;
                }
                foreach (var key in response)
                {
                    if (key.Value == null)
                    {
                        continue;
                    }

                    var descriptors = JimuHelper.Deserialize<byte[], List<JimuServiceRouteDesc>>(key.Value);
                    if (descriptors != null && descriptors.Any())
                    {
                        foreach (var descriptor in descriptors)
                        {
                            if (_routes.Any(x => x.ServiceDescriptor.Id == descriptor.ServiceDescriptor.Id))
                            {
                                continue;
                            }
                            List<JimuAddress> addresses = new List<JimuAddress>(descriptor.AddressDescriptors.ToArray().Count());
                            foreach (var addDesc in descriptor.AddressDescriptors)
                            {
                                //var addrType = Type.GetType(addDesc.Type);
                                addresses.Add(JimuHelper.Deserialize(addDesc.Value, typeof(JimuAddress)) as JimuAddress);
                            }

                            _routes.Add(new JimuServiceRoute
                            {
                                Address = addresses,
                                ServiceDescriptor = descriptor.ServiceDescriptor
                            });
                        }
                    }
                }
            }


            return _routes;
        }
    }
}
