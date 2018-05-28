using Microsoft.AspNetCore.Mvc;

namespace Jimu.ApiGateway.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}