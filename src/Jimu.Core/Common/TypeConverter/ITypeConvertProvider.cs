using System;

namespace Jimu
{
    public interface ITypeConvertProvider
    {
        object Convert(object instance, Type destinationType);
    }
}