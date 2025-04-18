using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.SeatType;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class SeatTypeController : BaseController
    {
        private readonly ISeatTypeService _seatTypeService;
        private readonly ILogger<SeatTypeController> _logger;

        public SeatTypeController(ISeatTypeService seatTypeService, ILogger<SeatTypeController> logger)
        {
            _seatTypeService = seatTypeService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("GetAllSeatTypes")]
        public async Task<IActionResult> GetAllSeatTypes()
        {
            var seatTypes = await _seatTypeService.GetAllSeatTypesAsync();
            return Json(seatTypes);
        }


        [HttpPost]
        [Route("GetSeatTypes")]
        public async Task<IActionResult> GetSeatTypes([FromForm] SeatTypeQueryDTO queryModel)
        {
            var result = await _seatTypeService.GetAllSeatTypesAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.SeatTypes
            });
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(long seatTypeId)
        {
            var result = await _seatTypeService.GetSeatTypeByIdAsync(seatTypeId);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return PartialView("_EditSeatType", result.Data);
        }

        [HttpPost]
        [Route("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] SeatTypeDTO seatType)
        {
            _logger.LogInformation("Name: " + seatType.Name);
            _logger.LogInformation("Price: " + seatType.PriceMultiplier);
            _logger.LogInformation("Color: " + seatType.Color);
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input data." });
            }
            var result = await _seatTypeService.UpdateSeatTypeAsync(seatType);

            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            return Json(new { success = true, message = result.Message });
        }

    }
}