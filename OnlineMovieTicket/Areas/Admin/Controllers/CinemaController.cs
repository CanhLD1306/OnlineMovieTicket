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
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;
        private readonly ILogger<CinemaController> _logger;

        public CinemaController(
            ICinemaService cinemaService, 
            ICountryService countryService,
            ICityService cityService, 
            ILogger<CinemaController> logger)
        {
            _cityService = cityService;
            _countryService = countryService;
            _cinemaService = cinemaService;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("GetCinemas")]  
        public async Task<IActionResult> GetCinemas([FromForm] CinemaQueryDTO queryModel)
        {
            _logger.LogInformation("CountryId: " + queryModel.CountryId);
            _logger.LogInformation("CityId: " + queryModel.CityId);
            var result = await _cinemaService.GetCinemasAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Cinemas
            });
        }

        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            var cinema = new CinemaDTO();
            ViewBag.Cities = new SelectList(await _cityService.GetAllCitiesAsync(0), "Id", "Name");
            return PartialView("_AddNewCinema", cinema);
        }

        [HttpPost] 
        [Route("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] CinemaDTO cinema)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _cinemaService.AddCinemaAsync(cinema);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(long id)
        {
            var result = await _cinemaService.GetCinemaByIdAsync(id);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            ViewBag.Countries = new SelectList(await _countryService.GetAllCountriesAsync(), "Id", "Name");
            ViewBag.Cities = new SelectList(await _cityService.GetAllCitiesAsync(0), "Id", "Name");
            return PartialView("_EditCinema", result.Data);
        }

        [HttpPost]
        [Route("Edit")]
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

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _cinemaService.GetCinemaByIdAsync(id);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_deleteCinema", result.Data);
        }

        [HttpPost]
        [Route("DeleteConfirm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm([FromForm] CinemaDTO cinema)
        {
            var result = await _cinemaService.DeleteCinemaAsync(cinema.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpPost]
        [Route("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(long id)
        {
            var result = await _cinemaService.ChangeStatusAsync(id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}