using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Cinema;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class CinemaController : BaseController
    {
        private readonly ICinemaService _cinemaService;
        private readonly ILogger<CinemaController> _logger;

        public CinemaController(
            ICinemaService cinemaService, 
            ILogger<CinemaController> logger)
        {
            _cinemaService = cinemaService;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetCinemasByCity")]
        public async Task<IActionResult> GetCinemasByCity(long cityId)
        {
            var cinemas = await _cinemaService.GetCinemasByCityAsync(cityId);
            return Json(cinemas);
        }

        [HttpPost("GetCinemas")]
        public async Task<IActionResult> GetCinemas([FromForm] CinemaQueryDTO queryModel)
        {
            var result = await _cinemaService.GetCinemasAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Cinemas
            });
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var cinema = new CinemaDTO();
            return PartialView("_CreateNewCinema", cinema);
        }

        [HttpPost("Create")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CinemaDTO cinema)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _cinemaService.CreateCinemaAsync(cinema);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(long cinemaId)
        {
            var result = await _cinemaService.GetCinemaByIdAsync(cinemaId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_EditCinema", result.Data);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] CinemaDTO cinema)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _cinemaService.UpdateCinemaAsync(cinema);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long cinemaId)
        {
            var result = await _cinemaService.GetCinemaByIdAsync(cinemaId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_deleteCinema", result.Data);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] CinemaDTO cinema)
        {
            var result = await _cinemaService.DeleteCinemaAsync(cinema.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(long cinemaId)
        {
            var result = await _cinemaService.ChangeStatusAsync(cinemaId);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}