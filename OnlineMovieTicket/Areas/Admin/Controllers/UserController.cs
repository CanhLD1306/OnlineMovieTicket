using Microsoft.AspNetCore.Mvc;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}