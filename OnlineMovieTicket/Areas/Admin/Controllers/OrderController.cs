using Microsoft.AspNetCore.Mvc;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}