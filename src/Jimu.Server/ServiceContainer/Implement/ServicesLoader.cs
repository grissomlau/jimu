using Jimu.Logger;
using Jimu.Server.ServiceContainer.Implement;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jimu.Server
{
    public class ServicesLoader : IDisposable
    {
        //readonly string _path;//where are the dll directory
        //readonly bool _enableWatchChanged;
        readonly ILogger _logger;
        volatile bool _loading;//is loading the dll
        volatile bool _isWatching;//is watching the path
        static object _lockObj = new object();
        readonly IServiceEntryContainer _serviceEntryContainer;
        //readonly string _watchingFilePattern;
        volatile bool _changingInloading;

        readonly ServiceOptions _options;
        readonly List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        /// <summary>
        /// dll loader
        /// </summary>
        /// <param name="serviceEntryContainer"></param>
        /// <param name="logger"></param>
        /// <param name="path">where are the dll directory</param>
        /// <param name="watchingFilePattern">what type of file will be watch when enableWatchChanged is true</param>
        /// <param name="enableWatchChanged">whether enable watch file changed</param>
        public ServicesLoader(IServiceEntryContainer serviceEntryContainer, ILogger logger, ServiceOptions options)
        {
            _logger = logger;
            _serviceEntryContainer = serviceEntryContainer;
            _options = options;

            //if (string.IsNullOrEmpty(_options.Path))
            //{
            //    _options.Path = "./";
            //}
        }


        public void LoadServices()
        {
            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                // avoid loading *.resources dlls, because of: https://github.com/dotnet/coreclr/issues/8416
                if (name.Name.EndsWith("resources"))
                {
                    return null;
                }

                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies)
                {
                    if (IsCandidateLibrary(library, name))
                    {
                        return context.LoadFromAssemblyName(new AssemblyName(library.Name));
                    }
                }

                var foundDlls = Directory.GetFileSystemEntries(new FileInfo(_options.Path).FullName, name.Name + ".dll", SearchOption.AllDirectories);
                if (foundDlls.Any())
                {
                    using (var sr = File.OpenRead(foundDlls[0]))
                    {
                        return context.LoadFromStream(sr);
                    }
                }

                _logger.Warn($"cannot found assembly {name.Name}, path: { _options.Path}");
                return null;
                //return context.LoadFromAssemblyName(name);
            };

            HashSet<Assembly> assemblies = new HashSet<Assembly>();
            var patterns = _options.LoadFilePattern.Split(',');
            foreach (var pattern in patterns)
            {
                string[] files;
                if (string.IsNullOrEmpty(_options.Path))
                {
                    files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, pattern.Trim());
                }
                else
                {
                    files = Directory.GetFiles(_options.Path, pattern.Trim());
                }
                foreach (var file in files)
                {
                    //var key = Path.GetFileName(file);

#if DEBUG
                    var path = _options.Path;
                    if (string.IsNullOrEmpty(path) || path == "./")
                    {
                        path = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    var myAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(path, file));
                    assemblies.Add(myAssembly);
#else

                    using (var sr = File.OpenRead(file))
                    {
                        //var myAssembly = AssemblyLoadContext.Default.LoadFromStream(sr);
                        var bytes = new byte[sr.Length];
                        sr.Read(bytes, 0, (int)sr.Length);
                        var myAssembly = Assembly.Load(bytes);
                        assemblies.Add(myAssembly);
                        //var types = myAssembly.ExportedTypes;
                        //foreach (var type in types)
                        //{
                        //    //list.AddOrUpdate(type.FullName, type, (x, t) => type);
                        //    _types.Add(type);
                        //}
                    }
#endif
                }
            }

            _serviceEntryContainer.LoadServices(assemblies.ToList());

            if (_changingInloading)
            {
                _changingInloading = false;
                this.LoadServices();
            }

            _loading = false;
            if (_options.EnableWatch && !_isWatching)
            {
                _isWatching = true;
                this.Watch();
            }

            if (!assemblies.Any())
            {
                _logger.Debug($"[config]loading services: Nothing to load.");
            }
            else
            {
                _logger.Debug($"[config]loading services: {string.Join(",", assemblies)}");
            }
            _logger.Info($"[config]loaded services.");
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, AssemblyName assemblyName)
        {
            return (library.Name == (assemblyName.Name))
                    || (library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name)));
        }
        private void Watch()
        {

            var filters = _options.WatchFilePattern.Split(',');
            foreach (var filter in filters)
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = _options.Path;

                // Watch for changes in LastAccess and LastWrite times, and
                // the renaming of files or directories.
                watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                // Only watch text files.
                //_watcher.Filter = "*.dll";
                watcher.Filter = filter;

                // Add event handlers.
                watcher.Changed += Watcher_Changed;
                watcher.Created += Watcher_Changed;
                watcher.Deleted += Watcher_Changed;
                watcher.Renamed += Watcher_Changed;

                // Begin watching.
                watcher.EnableRaisingEvents = true;
                _watchers.Add(watcher);
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _changingInloading = true;
            lock (_lockObj)
            {
                if (!_loading)
                {
                    _loading = true;
                    Thread.Sleep(10000);//waiting for changing finish
                    _changingInloading = false;
                    Task.Run(() => this.LoadServices());
                }
            }
        }

        private void ClearWatchs()
        {
            _watchers.ForEach(x =>
            {
                try
                {
                    x.Dispose();
                }
                catch { }
            });
            _watchers.Clear();

        }

        public void Dispose()
        {
            ClearWatchs();
        }
    }
}
