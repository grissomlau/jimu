using System.Collections.Generic;
using Jimu.ApiGateway.Model;
using Microsoft.AspNetCore.Mvc;

namespace Jimu.ApiGateway.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SysController : Controller
    {
        public List<Menu> Menus()
        {
            return new List<Menu>
            {
                new Menu{ Code= "Service", Name="Service", Seq=10,ParentCode = null },
                new Menu{ Code= "Server", Name="Server", Seq=10, Url = "/server/index.html",ParentCode = "Service" }
            };
        }
    }
}