using Jimu.Client.ApiGateway;
using Microsoft.Extensions.Hosting;
using System;

namespace Jimu.Client
{
    public static class ApplicationExtensions
    {
        /// <summary>
        /// webhost, run in Program.Main, e.g.: app.RunWebHost(hostBuilder=>{ // dosth});
        /// </summary>
        /// <param name="this"></param>
        /// <param name="build"></param>
        public static void RunWebHost(this IApplication @this, Action<IHostBuilder> build = null, string settingName = "JimuAppClientSettings")
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            build?.Invoke(hostBuilder);
            hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseJimu(settingName, @this);
            });
            hostBuilder.Build().Run();
        }

    }
}
