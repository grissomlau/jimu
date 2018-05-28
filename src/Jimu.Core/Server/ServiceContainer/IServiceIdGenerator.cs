using System.Reflection;

namespace Jimu.Core.Server.ServiceContainer
{
    public interface IServiceIdGenerator
    {
        string GenerateServiceId(MethodInfo method);
    }
}