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
        private readonly IShowtimeSeatService _showtimeSeatService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;
        private readonly ICinemaService _cinemaService;
        private readonly IMovieService _movieService;
        private readonly IRoomService _roomService;

        public MovieController(
            IShowtimeSeatService showtimeSeatService,
            ILogger<MovieController> logger, 
            IRoomService roomService,
            ICountryService countryService,
            ICityService cityService,
            ICinemaService cinemaService, 
            IMovieService movieService)
        {
            _showtimeSeatService = showtimeSeatService;
            _roomService = roomService;
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

        [HttpGet("GetShowtimes")]
        public async Task<IActionResult> GetShowtimes(long cinemaId, long movieId, DateTime selectedDate)
        {
            var result = await _roomService.GetRoomsWithShowtimes(cinemaId, movieId, selectedDate);
            return PartialView("_Showtimes", result);
        }

        [HttpGet("GetShowtimeSeats")]
        public async Task<IActionResult> GetShowtimeSeats(long showtimeId)
        {
            var result = await _showtimeSeatService.GetAllShowtimeSeatsByShowtimeAsync(showtimeId);
            return PartialView("_ShowtimeSeats", result);
        }
    }
}