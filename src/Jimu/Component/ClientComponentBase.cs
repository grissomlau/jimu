using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public abstract class ClientComponentBase : ComponentBase
    {
        public ClientComponentBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }
    }
}
