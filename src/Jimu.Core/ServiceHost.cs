using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Jimu
{
    public class ServiceHost : IServiceHost
    {
        private readonly List<Action<IContainer>> _disposeActions;
        private readonly List<Action<IContainer>> _runActions;

        public ServiceHost(List<Action<IContainer>> runActions = null, List<Action<IContainer>> disposeActions = null)
        {
            _disposeActions = disposeActions ?? new List<Action<IContainer>>();
            _runActions = runActions ?? new List<Action<IContainer>>();
        }

        public IContainer Container { get; set; }

        public void Dispose()
        {
            if (_disposeActions.Any())
                _disposeActions.ForEach(x => { x(Container); });
            Container.Dispose();
        }

        public void DisposeAction(Action<IContainer> action)
        {
            _disposeActions.Add(action);
        }

        public void RunAction(Action<IContainer> action)
        {
            _runActions.Add(action);
        }

        public void Run()
        {
            if (_runActions.Any())
                _runActions.ForEach(x => { x(Container); });
        }
    }
}