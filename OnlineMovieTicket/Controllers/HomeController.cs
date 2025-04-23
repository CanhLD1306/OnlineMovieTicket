using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Movie;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.ViewModels;

namespace OnlineMovieTicket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBannerService _bannerService;
        private readonly IMovieService _movieService;

        public HomeController(
            ILogger<HomeController> logger, 
            IMovieService movieService,
            IBannerService bannerService)
        {
            _movieService = movieService;
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
        
        [HttpGet("GetMoviesCommingSoon")]
        public async Task<IActionResult> GetMoviesCommingSoon()
        {
            var queryModel = new MovieQueryForUserDTO
            {
                IsCommingSoon = true,
                PageNumber = 1,
                PageSize = 4,
                SortBy = "ReleaseDate",
                IsDescending = true
            };
            var movies = await _movieService.GetMoviesForUserAsync(queryModel);
            return PartialView("_MoviesCoomingSoon", movies);

        }

        [HttpGet("GetMoviesNowShowing")]
        public async Task<IActionResult> GetMoviesNowShowing()
        {
            var queryModel = new MovieQueryForUserDTO
            {
                IsCommingSoon = false,
                PageNumber = 1,
                PageSize = 4,
                SortBy = "ReleaseDate",
                IsDescending = true
            };
            var movies = await _movieService.GetMoviesForUserAsync(queryModel);
            return PartialView("_MoviesNowShowing", movies);

        }
        
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
