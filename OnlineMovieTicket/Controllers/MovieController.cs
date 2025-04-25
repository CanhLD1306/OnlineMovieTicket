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
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly ICinemaService _cinemaService;
        private readonly IMovieService _movieService;

        public MovieController(
            
            ILogger<MovieController> logger, 
            ICountryService countryService,
            ICityService cityService,
            ICinemaService cinemaService, 
            IMovieService movieService)
        {
            _movieService = movieService;
            _countryService = countryService;
            _cityService = cityService;
            _cinemaService = cinemaService;
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

        [HttpGet("GetAllCountries")]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            return Json(countries);
        }

        [HttpGet("GetCitiesByCountry")]
        public async Task<IActionResult> GetCitiesByCountry(long countryId)
        {
            var cities = await _cityService.GetCitiesByCountryAsync(countryId);
            return Json(cities);
        }

        [HttpGet("GetCinemasByCity")]
        public async Task<IActionResult> GetCinemasByCity(long cityId)
        {
            var cinemas = await _cinemaService.GetCinemasByCityAsync(cityId);
            return Json(cinemas);
        }

    }
}