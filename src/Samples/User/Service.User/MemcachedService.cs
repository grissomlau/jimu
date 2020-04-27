using Enyim.Caching;
using IService.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.User
{
    public class MemcachedService : IMemcachedService
    {
        private IMemcachedClient _client;
        public MemcachedService(IMemcachedClient client)
        {
            _client = client;
        }
        public string Get(string key)
        {
            return _client.Get<string>(key);
        }

        public void Set(string key, string value)
        {
            _client.Add(key, value, 100);
        }
    }
}
