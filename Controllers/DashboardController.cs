using Microsoft.AspNetCore.Mvc;

namespace Nomad.Controllers
{
    public class DashboardController : Controller
    {
        // GET /
        public IActionResult Index()
        {
            return View("~/Views/Nomad/Index.cshtml");
        }
    }
}