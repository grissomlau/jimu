using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public abstract class ServerComponentBase : ComponentBase
    {
        public ServerComponentBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }

        /// <summary>
        /// when custom's service register
        /// </summary>
        /// <param name="serviceContainerBuilder"></param>
        /// <param name="jimuAppSettings"></param>
        public virtual void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {

        }

        /// <summary>
        /// when custom's service has been read
        /// </summary>
        /// <param name="container"></param>
        public virtual void DoServiceInit(IContainer container)
        {

        }

    }
}
