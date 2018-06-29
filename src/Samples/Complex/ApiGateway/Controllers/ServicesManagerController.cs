using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Jimu.Client;
using Jimu.Client.ApiGateway;
using Microsoft.AspNetCore.Mvc;

namespace Jimu.ApiGateway.Controllers
{
    //[Produces("application/json")]
    public class ServicesManagerController : Controller
    {
        //[HttpGet(Name ="addresses")]
        public async Task<List<JimuAddress>> GetAddresses()
        {
            var serviceDiscovery = JimuClient.Host.Container.Resolve<IClientServiceDiscovery>();
            var addresses = await serviceDiscovery.GetAddressAsync();
            return addresses;
            //return (from addr in addresses
            //select addr.ToString()).ToArray();

        }

        //[HttpGet(Name ="services")]
        public async Task<List<JimuServiceDesc>> GetServices(string server)
        {
            var serviceDiscovery = JimuClient.Host.Container.Resolve<IClientServiceDiscovery>();
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