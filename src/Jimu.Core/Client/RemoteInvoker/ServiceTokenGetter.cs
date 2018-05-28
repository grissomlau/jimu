using System;

namespace Jimu.Core.Client.RemoteInvoker
{
    public class ServiceTokenGetter : IServiceTokenGetter
    {
        public Func<string> GetToken { get; set; }
    }
}