using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class DashboardController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
