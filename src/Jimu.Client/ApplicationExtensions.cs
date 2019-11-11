using Autofac;
using Autofac.Extensions.DependencyInjection;
using Jimu.Client.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        public static void RunWebHost(this IApplication @this, string settingName = "JimuAppClientSettings")
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            //hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            //{
            //    @this.BeforeRunAction(null);
            //    webBuilder.UseJimu(settingName, @this);
            //});
            hostBuilder.Build().Run();
        }

        public static void WebHostConfigure(this IHostBuilder @this, Action<IApplicationBuilder> action)
        {

        }

    }
}
