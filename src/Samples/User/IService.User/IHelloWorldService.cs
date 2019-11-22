using Jimu;
using System;

namespace IService.User
{
    /// <summary>
    /// hello world
    /// </summary>
    [Jimu("/{Service}")]
    public interface IHelloWorldService : IJimuService
    {
        /// <summary>
        /// get something
        /// </summary>
        /// <returns></returns>
        [JimuGet(true)]
        string Get();
    }
}
