using Microsoft.AspNetCore.Mvc;


namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class MovieController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}