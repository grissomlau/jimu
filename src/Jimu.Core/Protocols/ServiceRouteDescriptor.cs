using System.Collections.Generic;

namespace Jimu.Core.Protocols
{
    public class ServiceRouteDescriptor
    {
        public IEnumerable<ServiceAddressDescriptor> AddressDescriptors { get; set; }

        public ServiceDescriptor ServiceDescriptor { get; set; }
    }
}