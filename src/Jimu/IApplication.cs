using Autofac;
using Microsoft.Extensions.Configuration;
using System;

namespace Jimu
{
    public interface IApplication : IDisposable
    {
        /// <summary>
        ///     autofac container
        /// </summary>
        IContainer Container { get; }

        IConfigurationRoot JimuAppSettings { get; }

        /// <summary>
        ///     delegate will be execute in host disposing
        /// </summary>
        /// <param name="action"></param>
        IApplication DisposeAction(Action<IContainer> action);
        IApplication RunAction(Action<IContainer> action);
        IApplication BeforeRunAction(Action<IContainer> action);
        void Run();
    }
}