using Jimu;
using System;

namespace User.IService
{
    /// <summary>
    /// hello world
    /// </summary>
    [JimuServiceRoute("/{Service}")]
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
