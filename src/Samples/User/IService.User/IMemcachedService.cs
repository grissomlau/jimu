using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace IService.User
{
    /// <summary>
    /// try memcached
    /// </summary>
    [Jimu("/{Service}")]
    public interface IMemcachedService : IJimuService
    {
        [JimuPost(true)]
        void Set(string key, string value);
        [JimuGet(true)]
        string Get(string key);
    }
}
