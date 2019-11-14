using Autofac;
using Jimu.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client
{
    public class ClientGeneralModuleBase : ClientModuleBase
    {
        public ClientGeneralModuleBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }
        public virtual void DoHostBuild(IHostBuilder hostBuilder, IContainer container)
        {
        }
    }
}
