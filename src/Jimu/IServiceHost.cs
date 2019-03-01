using System;
using Autofac;

namespace Jimu
{
    public interface IServiceHost : IDisposable
    {
        /// <summary>
        ///     autofac container
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        ///     delegate will be execute in host disposing
        /// </summary>
        /// <param name="action"></param>
        void DisposeAction(Action<IContainer> action);

        void RunAction(Action<IContainer> action);

        void Run();
    }
}