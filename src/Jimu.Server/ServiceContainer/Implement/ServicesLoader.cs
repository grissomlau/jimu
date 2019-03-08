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
        readonly string _path;//where are the dll directory
        readonly bool _enableWatchChanged;
        readonly ILogger _logger;
        FileSystemWatcher _watcher;
        volatile bool _loading;//is loading the dll
        volatile bool _isWatching;//is watching the path
        static object _lockObj = new object();
        readonly IServiceEntryContainer _serviceEntryContainer;
        readonly string _watchingFilePattern;

        /// <summary>
        /// dll loader
        /// </summary>
        /// <param name="serviceEntryContainer"></param>
        /// <param name="logger"></param>
        /// <param name="path">where are the dll directory</param>
        /// <param name="watchingFilePattern">what type of file will be watch when enableWatchChanged is true</param>
        /// <param name="enableWatchChanged">whether enable watch file changed</param>
        public ServicesLoader(IServiceEntryContainer serviceEntryContainer, ILogger logger, string path, string watchingFilePattern = "*.dll", bool enableWatchChanged = true)
        {
            _path = path;
            _enableWatchChanged = enableWatchChanged;
            _logger = logger;
            _serviceEntryContainer = serviceEntryContainer;
            _watchingFilePattern = watchingFilePattern;
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

                var foundDlls = Directory.GetFileSystemEntries(new FileInfo(_path).FullName, name.Name + ".dll", SearchOption.AllDirectories);
                if (foundDlls.Any())
                {
                    using (var sr = File.OpenRead(foundDlls[0]))
                    {
                        return context.LoadFromStream(sr);
                    }
                }

                _logger.Warn($"cannot found assembly {name.Name}, path: { _path}");
                return null;
                //return context.LoadFromAssemblyName(name);
            };

            HashSet<Assembly> assemblies = new HashSet<Assembly>();
            foreach (var file in Directory.GetFiles(_path, "*.dll"))
            {
                var key = Path.GetFileName(file);
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
            }

            _serviceEntryContainer.LoadServices(assemblies.ToList());

            _loading = false;
            if (_enableWatchChanged && !_isWatching)
            {
                _isWatching = true;
                this.Watch();
            }

            _logger.Info($"[config]loaded services: {string.Join(",", assemblies)}");
        }

        private static bool IsCandidateLibrary(RuntimeLibrary library, AssemblyName assemblyName)
        {
            return (library.Name == (assemblyName.Name))
                    || (library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name)));
        }
        private void Watch()
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = _path;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            _watcher.NotifyFilter = NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Only watch text files.
            //_watcher.Filter = "*.dll";
            _watcher.Filter = this._watchingFilePattern;

            // Add event handlers.
            _watcher.Changed += Watcher_Changed;
            _watcher.Created += Watcher_Changed;
            _watcher.Deleted += Watcher_Changed;
            _watcher.Renamed += Watcher_Changed;

            // Begin watching.
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (_lockObj)
            {
                if (!_loading)
                {
                    _loading = true;
                    Thread.Sleep(10000);//waiting for changing finish
                    Task.Run(() => this.LoadServices());
                }
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }
        }
    }
}
