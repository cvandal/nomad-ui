using Microsoft.AspNetCore.Mvc;

namespace Nomad.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            return View("~/Views/Shared/Error.cshtml", statusCode);
        }
    }
}
