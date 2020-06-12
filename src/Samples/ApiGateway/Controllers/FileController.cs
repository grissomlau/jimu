using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    //[Authorize]
    public class FileController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Hi()
        {
            return View();
        }
    }
}
