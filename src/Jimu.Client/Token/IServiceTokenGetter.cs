using System;

namespace Jimu.Client.Token
{

    public interface IServiceTokenGetter
    {
        /// <summary>
        ///     get token
        /// </summary>
        Func<string> GetToken { get; set; }
    }
}