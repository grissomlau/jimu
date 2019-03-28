using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public abstract class ClientModuleBase : ModuleBase
    {
        public ClientModuleBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }
    }
}
