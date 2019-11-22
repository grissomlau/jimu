using Jimu.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Jimu.Server.ServiceContainer.Implement
{
    public class ServicesLoader : IDisposable
    {
        readonly ILogger _logger;
        readonly IServiceEntryContainer _serviceEntryContainer;
        readonly ServiceOptions _options;

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
        }


        public void LoadServices()
        {
            HashSet<Assembly> assemblies = new HashSet<Assembly>();
            var patterns = _options.LoadFilePattern.Split(',');
            foreach (var pattern in patterns)
            {
                var path = _options.Path;
                if (string.IsNullOrEmpty(path) || path == "./")
                {
                    path = AppDomain.CurrentDomain.BaseDirectory;
                }
                string[] files = Directory.GetFiles(path, pattern.Trim());
                foreach (var file in files)
                {
                    var myAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(path, file));
                    assemblies.Add(myAssembly);
                }
            }

            _serviceEntryContainer.LoadServices(assemblies.ToList());

            if (!assemblies.Any())
            {
                _logger.Debug($"[config]loaded services: Nothing to load.");
            }
            else
            {
                _logger.Debug($"[config]loaded services: {string.Join(",", assemblies)}");
            }
        }

        public void Dispose()
        {
        }
    }
}
