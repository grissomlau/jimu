using System;

namespace Jimu.Client
{

    public interface IServiceTokenGetter
    {
        /// <summary>
        ///     get token
        /// </summary>
        Func<string> GetToken { get; set; }
    }
}