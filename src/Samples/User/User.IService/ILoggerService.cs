using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace User.IService
{
    /// <summary>
    /// try logger
    /// </summary>
    [JimuServiceRoute("/{Service}")]
    public interface ILoggerService : IJimuService
    {
        /// <summary>
        /// post something to log
        /// </summary>
        /// <param name="log">something to log</param>
        [JimuPost(true)]
        void Post(string log);
    }
}
