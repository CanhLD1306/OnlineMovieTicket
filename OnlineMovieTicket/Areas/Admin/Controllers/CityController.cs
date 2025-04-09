using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class CityController : BaseController
    {
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;
        private readonly ILogger<CityController> _logger;

        public CityController(ICityService cityService, ICountryService countryService, ILogger<CityController> logger)
        {
            _countryService = countryService;
            _cityService = cityService;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.Countries = await _countryService.GetAllCountriesAsync();
            return View();
        }

        [HttpPost]
        [Route("GetCities")]  
        public async Task<IActionResult> GetCities([FromForm] CityQueryDTO queryModel)
        {
            var result = await _cityService.GetCitiesAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Cities
            });
        }

        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            var city = new CityDTO();
            ViewBag.Countries = new SelectList(await _countryService.GetAllCountriesAsync(), "Id", "Name");
            return PartialView("_AddNewCity", city);
        }

        [HttpPost] 
        [Route("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] CityDTO city)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _cityService.AddCityAsync(city);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(long id)
        {
            var result = await _cityService.GetCityByIdAsync(id);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            ViewBag.Countries = new SelectList(await _countryService.GetAllCountriesAsync(), "Id", "Name");
            return PartialView("_EditCity", result.Data);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] CityDTO city)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _cityService.UpdateCityAsync(city);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _cityService.GetCityByIdAsync(id);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_deleteCity", result.Data);
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm([FromForm] CityDTO city)
        {
            var result = await _cityService.DeleteCityAsync(city.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}