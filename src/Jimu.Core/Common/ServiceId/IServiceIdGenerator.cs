using System.Reflection;

namespace Jimu
{
    public interface IServiceIdGenerator
    {
        string GenerateServiceId(MethodInfo method);
    }
}