using Microsoft.AspNetCore.Mvc;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class RoomController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}