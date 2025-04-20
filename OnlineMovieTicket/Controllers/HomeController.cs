using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.ViewModels;

namespace OnlineMovieTicket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBannerService _bannerService;

        public HomeController(ILogger<HomeController> logger, IBannerService bannerService)
        {
            _bannerService = bannerService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("GetBanners")]
        public async Task<IActionResult> GetBanners()
        {
            var banners = await _bannerService.GetActiveBannersAsync();
            return PartialView("_Banners", banners!.ToList());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
