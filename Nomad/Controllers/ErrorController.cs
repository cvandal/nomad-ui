using Microsoft.AspNetCore.Mvc;

namespace Nomad.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            return View("~/Views/Nomad/Error.cshtml", statusCode);
        }
    }
}
