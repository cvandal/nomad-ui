using Microsoft.AspNetCore.Mvc;

namespace Nomad.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error")]
        public IActionResult Jobs()
        {
            return View("~/Views/Nomad/Error.cshtml");
        }
    }
}
