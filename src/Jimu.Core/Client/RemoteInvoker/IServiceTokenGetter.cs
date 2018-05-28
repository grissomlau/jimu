using System;

namespace Jimu.Core.Client.RemoteInvoker
{
    /// <summary>
    ///     how to get the token
    /// </summary>
    public interface IServiceTokenGetter
    {
        /// <summary>
        ///     get token
        /// </summary>
        Func<string> GetToken { get; set; }
    }
}