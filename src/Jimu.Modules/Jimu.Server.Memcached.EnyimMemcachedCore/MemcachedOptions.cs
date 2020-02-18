using Enyim.Caching.Configuration;
using System;

namespace Jimu.Server.Memcached.EnyimMemcachedCore
{
    public class MemcachedOptions : MemcachedClientOptions
    {
        public bool Enable { get; set; } = true;
    }
}
