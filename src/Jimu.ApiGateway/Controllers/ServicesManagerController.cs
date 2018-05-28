using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Jimu.ApiGateway.Utils;
using Jimu.Core.Commons.Discovery;
using Jimu.Core.Protocols;
using Microsoft.AspNetCore.Mvc;

namespace Jimu.ApiGateway.Controllers
{
    //[Produces("application/json")]
    public class ServicesManagerController : Controller
    {
        //[HttpGet(Name ="addresses")]
        public async Task<List<Address>> GetAddresses()
        {
            var serviceDiscovery = JimuServiceProvider.Container.Resolve<IServiceDiscovery>();
            var addresses = await serviceDiscovery.GetAddressAsync();
            return addresses;
            //return (from addr in addresses
            //select addr.ToString()).ToArray();

        }

        //[HttpGet(Name ="services")]
        public async Task<List<ServiceDescriptor>> GetServices(string server)
        {
            var serviceDiscovery = JimuServiceProvider.Container.Resolve<IServiceDiscovery>();
            var routes = await serviceDiscovery.GetRoutesAsync();
            if (routes != null && routes.Any() && !string.IsNullOrEmpty(server))
            {
                return (from route in routes
                        where route.Address.Any(x => x.Code == server)
                        select route.ServiceDescriptor).ToList();
            }
            return (from route in routes select route.ServiceDescriptor).ToList();

        }

        public IActionResult Server()
        {
            return View();
        }

        public IActionResult Service()
        {
            return View();
        }

    }
}