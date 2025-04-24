using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Showtime;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class ShowtimeController : BaseController
    {
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<ShowtimeController> _logger;

        public ShowtimeController(IShowtimeService showtimeService, ILogger<ShowtimeController> logger)
        {
            _showtimeService = showtimeService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetShowtimes")]
        public async Task<IActionResult> GetShowtimes([FromForm] ShowtimeQueryDTO queryModel)
        {
            var result = await _showtimeService.GetShowtimesAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Showtimes
            });
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var model = new ShowtimeDTO();
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ShowtimeDTO showtime)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _showtimeService.CreateShowtimeAsync(showtime);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(long showtimeId)
        {
            var result = await _showtimeService.GetShowtimeWithSeatsById(showtimeId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return View(result.Data);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] ShowtimeWithSeatsDTO showtimeWithSeatsDTO)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _showtimeService.UpdateShowtimeAsync(showtimeWithSeatsDTO);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long showtimeId)
        {
            var result = await _showtimeService.GetShowtimeByIdAsync(showtimeId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_DeleteShowtime", result.Data);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] ShowtimeDTO showtime)
        {
            var result = await _showtimeService.DeleteShowtimeAsync(showtime.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}