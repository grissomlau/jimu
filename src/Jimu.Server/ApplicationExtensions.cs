using Autofac;
using Microsoft.Extensions.Hosting;
using System;

namespace Jimu.Server
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// just general console host
        /// </summary>
        /// <param name="this"></param>
        /// <param name="build"></param>
        public static void RunGeneralHost(this IApplication @this, Action<IHostBuilder, IContainer> build = null)
        {
            var hostBuilder = new HostBuilder();
            build?.Invoke(hostBuilder, @this.Container);
            @this.Run();
            hostBuilder.Build().Run();
        }

    }
}
