using Autofac;
using Jimu;
using Jimu.Client;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Modules.Auth
{
    public class RoleAuthModule : ClientModuleBase
    {
        private readonly RoleAuthOptions _options;
        public RoleAuthModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(RoleAuthOptions).Name).Get<RoleAuthOptions>();
        }
        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use RoleAuth for Api Auth");

                var caller = container.Resolve<IRemoteServiceCaller>();
                caller.UseMiddleware<RoleAuthMiddleware>(_options);
            }
            base.DoInit(container);
        }
    }
}
