using Autofac;
using Microsoft.Extensions.Configuration;

namespace Jimu.Module
{
    public abstract class ServerModuleBase : ModuleBase
    {
        public ServerModuleBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
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
        /// when custom's service has been ready
        /// </summary>
        /// <param name="container"></param>
        public virtual void DoServiceInit(IContainer container)
        {

        }

    }
}
