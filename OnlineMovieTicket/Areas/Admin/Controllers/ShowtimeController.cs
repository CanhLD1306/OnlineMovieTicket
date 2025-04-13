using Microsoft.AspNetCore.Mvc;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class ShowtimeController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}