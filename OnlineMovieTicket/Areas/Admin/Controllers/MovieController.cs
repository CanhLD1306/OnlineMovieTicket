using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Movie;
using OnlineMovieTicket.BL.Interfaces;


namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class MovieController : BaseController
    {
        private readonly IMovieService _movieService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost("GetMovies")]
        public async Task<IActionResult> GetMovies([FromForm] MovieQueryDTO queryModel)
        {
            var result = await _movieService.GetMoviesAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Movies
            });
        }

        [HttpGet("GetAllMovies")]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            return Json(movies);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var model = new MovieDTO();
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] MovieDTO movie)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _movieService.CreateMovieAsync(movie);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(long movieId)
        {
            var result = await _movieService.GetMovieByIdAsync(movieId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return View(result.Data);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] MovieDTO movie)
        {
            _logger.LogInformation("I'm Edit");
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _movieService.UpdateMovieAsync(movie);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long movieId)
        {
            var result = await _movieService.GetMovieByIdAsync(movieId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_DeleteMovie", result.Data);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] MovieDTO movie)
        {
            var result = await _movieService.DeleteMovieAsync(movie.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}