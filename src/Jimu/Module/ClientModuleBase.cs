using Microsoft.Extensions.Configuration;

namespace Jimu.Module
{
    public abstract class ClientModuleBase : ModuleBase
    {
        public ClientModuleBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }
    }
}
