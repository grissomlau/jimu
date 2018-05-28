using System;

namespace Jimu.Core.Server.ServiceContainer
{
    public interface ITypeConvertProvider
    {
        object Convert(object instance, Type destinationType);
    }
}