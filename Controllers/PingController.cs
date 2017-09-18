using Microsoft.AspNetCore.Mvc;

namespace Nomad.Controllers
{
    public class PingController : Controller
    {
        public IActionResult Index()
        {
            return Content("pong");
        }
    }
}
