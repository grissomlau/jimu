using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jimu
{
    public class Application : IApplication
    {
        private readonly List<Action<IContainer>> _disposeActions;
        private readonly List<Action<IContainer>> _runActions;
        private readonly List<Action<IContainer>> _beforeRunActions;

        public Application(List<Action<IContainer>> beforeRunActions = null, List<Action<IContainer>> runActions = null, List<Action<IContainer>> disposeActions = null)
        {
            _disposeActions = disposeActions ?? new List<Action<IContainer>>();
            _runActions = runActions ?? new List<Action<IContainer>>();
            _beforeRunActions = beforeRunActions ?? new List<Action<IContainer>>();
        }

        public IContainer Container { get; set; }

        public void Dispose()
        {
            if (_disposeActions.Any())
                _disposeActions.ForEach(x => { x(Container); });
            Container.Dispose();
        }

        public IApplication DisposeAction(Action<IContainer> action)
        {
            _disposeActions.Add(action);
            return this;
        }

        public IApplication RunAction(Action<IContainer> action)
        {
            _runActions.Add(action);
            return this;
        }

        public void Run()
        {
            if (_beforeRunActions.Any())
                _beforeRunActions.ForEach(x => x(Container));
            if (_runActions.Any())
                _runActions.ForEach(x => { x(Container); });
        }

        public IApplication BeforeRunAction(Action<IContainer> action)
        {
            _beforeRunActions.Add(action);
            return this;
        }
    }
}