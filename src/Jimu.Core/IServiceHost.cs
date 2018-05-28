using System;
using Autofac;

namespace Jimu.Core
{
    public interface IServiceHost : IDisposable
    {
        /// <summary>
        ///     autofac container
        /// </summary>
        IContainer Container { get; }

        /// <summary>
        ///     delegate will be exute in host disposing
        /// </summary>
        /// <param name="action"></param>
        void DisposeAction(Action<IContainer> action);

        void RunAction(Action<IContainer> action);

        void Run();
    }
}