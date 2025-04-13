using Microsoft.AspNetCore.Mvc;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class SeatTypeController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}