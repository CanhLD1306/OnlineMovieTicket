using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Country;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetAllCountries")]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _countryService.GetAllCountriesAsync();
            return Json(countries);
        }


        [HttpPost("GetCountries")]
        public async Task<IActionResult> GetCountries([FromForm] CountryQueryDTO queryModel)
        {
            var result = await _countryService.GetCountriesAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Countries
            });
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            var country = new CountryDTO();
            return PartialView("_CreateNewCountry", country);
        }

        [HttpPost("Create")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CountryDTO country)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _countryService.CreateCountryAsync(country);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(long countryId)
        {
            var result = await _countryService.GetCountryByIdAsync(countryId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_EditCountry", result.Data);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] CountryDTO country)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _countryService.UpdateCountryAsync(country);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long countryId)
        {
            var result = await _countryService.GetCountryByIdAsync(countryId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_DeleteCountry", result.Data);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] CountryDTO country)
        {
            var result = await _countryService.DeleteCountryAsync(country.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}