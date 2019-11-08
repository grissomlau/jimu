using System;

namespace Jimu
{
    public abstract class JimuServiceDescAttribute : Attribute
    {
        public abstract void Apply(JimuServiceDesc descriptor);
    }
}