using Microsoft.Extensions.Configuration;

namespace Jimu
{
    public abstract class ClientModuleBase : ModuleBase
    {
        public ClientModuleBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }
    }
}
