using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.Areas.Admin.ViewModel;
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

        [HttpPost]
        [Route("GetCountries")]  
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

        [HttpGet]
        [Route("Add")]
        public IActionResult Add()
        {
            var country = new CountryDTO();
            return PartialView("_AddNewCountry", country);
        }

        [HttpPost] 
        [Route("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] CountryDTO country)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _countryService.AddCountryAsync(country);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(long id)
        {
            var result = await _countryService.GetCountryByIdAsync(id);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_EditCountry", result.Data);
        }

        [HttpPost]
        [Route("Edit")]
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

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _countryService.GetCountryByIdAsync(id);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_deleteCountry", result.Data);
        }

        [HttpPost]
        [Route("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm([FromForm] CountryDTO country)
        {
            var result = await _countryService.DeleteCountryAsync(country.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}