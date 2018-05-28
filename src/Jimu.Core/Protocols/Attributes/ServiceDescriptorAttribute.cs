using System;

namespace Jimu.Core.Protocols.Attributes
{
    public abstract class ServiceDescriptorAttribute : Attribute
    {
        public abstract void Apply(ServiceDescriptor descriptor);
    }
}