using Autofac;
using Microsoft.Extensions.Configuration;

namespace Jimu.Module
{
    public enum ModuleExecPriority
    {
        /// <summary>
        ///  execute first
        /// </summary>
        Critical = 0,
        /// <summary>
        /// after Critical
        /// </summary>
        Important = 1,
        /// <summary>
        /// after Important
        /// </summary>
        Normal = 2,
        /// <summary>
        ///  last
        /// </summary>
        Low = 3
    }

    public abstract class ModuleBase
    {
        public virtual ModuleExecPriority Priority => ModuleExecPriority.Important;
        protected IConfigurationRoot JimuAppSettings { get; }
        public ModuleBase(IConfigurationRoot jimuAppSettings)
        {
            this.JimuAppSettings = jimuAppSettings;
        }

        /// <summary>
        /// when on jimu register 
        /// </summary>
        /// <param name="componentContainerBuilder"></param>
        /// <param name="jimuAppSettings"></param>
        public virtual void DoRegister(ContainerBuilder componentContainerBuilder)
        {

        }

        /// <summary>
        /// when on jimu init
        /// </summary>
        /// <param name="container"></param>
        /// <param name="jimuAppSettings"></param>
        public virtual void DoInit(IContainer container)
        {

        }

        /// <summary>
        /// when on jimu run
        /// </summary>
        /// <param name="container"></param>
        /// <param name="jimuAppSettings"></param>
        public virtual void DoRun(IContainer container)
        {

        }

        /// <summary>
        /// when on jimu dispose
        /// </summary>
        /// <param name="container"></param>
        public virtual void DoDispose(IContainer container)
        {
        }
    }
}
