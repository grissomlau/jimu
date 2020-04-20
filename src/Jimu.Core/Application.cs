using Autofac;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jimu
{
    public class Application : IApplication
    {
        private List<Action<IContainer>> _disposeActions;
        private List<Action<IContainer>> _runActions;
        private List<Action<IContainer>> _beforeRunActions;

        public Application(List<Action<IContainer>> beforeRunActions = null, List<Action<IContainer>> runActions = null, List<Action<IContainer>> disposeActions = null)
        {
            _disposeActions = disposeActions ?? new List<Action<IContainer>>();
            _runActions = runActions ?? new List<Action<IContainer>>();
            _beforeRunActions = beforeRunActions ?? new List<Action<IContainer>>();
        }

        public IContainer Container { get; set; }

        public IConfigurationRoot JimuAppSettings { get; set; }

        public void Dispose()
        {
            // dispose all resource: management and not management
            Dispose(true);
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
        ~Application()
        {
            // dispose not management resource
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposeActions.Any())
            {
                _disposeActions.ForEach(x =>
                {
                    x(Container);
                });
            }

            if (disposing)
            {
                _disposeActions = null;
                _runActions = null;
                _beforeRunActions = null;
            }

            Container.Dispose();
        }
    }
}