using Jimu.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Jimu.Client
{
    public class ClientGeneralModuleBase : ClientModuleBase
    {
        public ClientGeneralModuleBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }
        public virtual void DoHostBuild(IHostBuilder hostBuilder)
        {
        }
    }
}
