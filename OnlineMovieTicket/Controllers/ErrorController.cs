using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OnlineMovieTicket.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/500")]
        public IActionResult Error500()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var error = exceptionHandlerPathFeature?.Error;
            ViewData["Message"] = "Something went wrong on the server.";
            return View("Error500");
        }

        [Route("Error/{code:int}")]
        public IActionResult HandleErrorCode(int code)
        {
            string message = code switch
            {
                404 => "Oops! Page not found.",
                403 => "Access denied.",
                401 => "Unauthorized access.",
                _ => "Something went wrong."
            };
            ViewData["Code"] = code;
            ViewData["Message"] = message;
            return View("StatusCode");
        }
    }
}