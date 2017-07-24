using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Nomad.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("/error")]
        public IActionResult ExceptionHandler()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            ViewData["StatusCode"] = HttpContext.Response.StatusCode;
            ViewData["Message"] = exception.Error.Message;
            ViewData["StackTrace"] = exception.Error.StackTrace;

            return View("~/Views/Error.cshtml");
        }

        [HttpGet("/error/{0}")]
        public IActionResult StatusCodeHandler()
        {
            ViewData["StatusCode"] = HttpContext.Response.StatusCode;

            return View("~/Views/Error.cshtml");
        }
    }
}
