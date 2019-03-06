using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Autofac;
using Autofac.Core;
using Jimu.Server.Implement.Parser;

namespace Jimu.Server
{
    public class ServiceEntryContainer : IServiceEntryContainer
    {
        private readonly IContainer _container;
        private readonly List<JimuServiceEntry> _services;


        public ServiceEntryContainer(IContainer container)
        {
            _container = container;
            _services = new List<JimuServiceEntry>();
        }


        public IServiceEntryContainer AddServices(Type[] types)
        {
            var serviceTypes = types
                .Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).Distinct();

            JimuServiceDescParser descParser = new JimuServiceDescParser();
            JimuServiceEntryParser entryParser = new JimuServiceEntryParser(_container);
            foreach (var type in serviceTypes)
            {
                foreach (var methodInfo in type.GetTypeInfo().GetMethods().Where(x => x.GetCustomAttributes<JimuServiceDescAttribute>().Any()))
                {

                    JimuServiceDesc desc = descParser.Parse(methodInfo);
                    var service = entryParser.Parse(methodInfo,desc);
                    _services.Add(service);
                }
            }

            return this;
        }

        public List<JimuServiceEntry> GetServiceEntry()
        {
            return _services;
        }


    }
}