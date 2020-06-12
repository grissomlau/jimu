using Jimu.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Jimu.Server
{
    public class ServerGeneralModuleBase : ServerModuleBase
    {
        public ServerGeneralModuleBase(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }

        public virtual void DoHostBuild(IHostBuilder hostBuilder)
        {

        }
    }
}
