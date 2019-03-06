using System;

namespace Jimu.Client
{
    /// <summary>
    ///     how to get the token
    /// </summary>
    public class ServiceTokenGetter : IServiceTokenGetter
    {
        public Func<string> GetToken { get; set; }
    }
}