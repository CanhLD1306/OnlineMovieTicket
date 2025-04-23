using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Movie;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.ViewModels;

namespace OnlineMovieTicket.Controllers
{
    public class MovieController : Controller
    {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieService _movieService;

        public MovieController(
            ILogger<MovieController> logger, 
            IMovieService movieService)
        {
            _movieService = movieService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetMovies")]
        public async Task<IActionResult> GetMovies([FromQuery] MovieQueryForUserDTO queryModel)
        {
            var result = await _movieService.GetMoviesForUserAsync(queryModel);
            return PartialView("_MoviesList", result);
        }

        public async Task<IActionResult> Details(long movieId)
        {
            var result = await _movieService.GetMovieByIdAsync(movieId);
            return View(result.Data);
        }
    
    }
}