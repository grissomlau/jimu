using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public abstract class ModuleBase
    {
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
        /// when on jimu run before
        /// </summary>
        /// <param name="container"></param>
        /// <param name="jimuAppSettings"></param>
        public virtual void DoBeforeRun(IContainer container)
        {

        }
    }
}
