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
        private readonly ILogger<CityController> _logger;

        public CityController(ICityService cityService, ICountryService countryService, ILogger<CityController> logger)
        {
            _cityService = cityService;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetCitiesByCountry")]
        public async Task<IActionResult> GetCitiesByCountry(long countryId)
        {
            var cities = await _cityService.GetCitiesByCountryAsync(countryId);
            return Json(cities);
        }

        [HttpPost("GetCities")]
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

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var city = new CityDTO();
            return PartialView("_CreateNewCity", city);
        }

        [HttpPost("Create")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CityDTO city)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _cityService.CreateCityAsync(city);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(long cityId)
        {
            var result = await _cityService.GetCityByIdAsync(cityId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_EditCity", result.Data);
        }

        [HttpPost("Edit")]
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

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long cityId)
        {
            var result = await _cityService.GetCityByIdAsync(cityId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_DeleteCity", result.Data);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] CityDTO city)
        {
            var result = await _cityService.DeleteCityAsync(city.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}