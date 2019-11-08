using System.Collections.Generic;

namespace Jimu
{
    public class JimuServiceRouteDesc
    {
        public IEnumerable<JimuAddressDesc> AddressDescriptors { get; set; }

        public JimuServiceDesc ServiceDescriptor { get; set; }
    }
}