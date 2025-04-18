using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.Banner;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class BannerController : BaseController
    {
        private readonly IBannerService _bannerService;
        private readonly ILogger<BannerController> _logger;

        public BannerController(IBannerService bannerService, ILogger<BannerController> logger)
        {
            _bannerService = bannerService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetBanners")]
        public async Task<IActionResult> GetBanners([FromForm] BannerQueryDTO queryModel)
        {
            var result = await _bannerService.GetBannersAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Banners
            });
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var banner = new BannerDTO();
            return PartialView("_CreateNewBanner", banner);
        }

        [HttpPost("Create")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BannerDTO banner)
        {
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }

            var result = await _bannerService.CreateBannerAsync(banner);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }

            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(long bannerId)
        {
            var result = await _bannerService.GetBannerByIdAsync(bannerId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_EditBanner", result.Data);
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] BannerDTO banner)
        {
            _logger.LogInformation("I'm Edit");
            if(!ModelState.IsValid){
                return Json(new {success = false, message = "Invalid input data."});
            }
            var result = await _bannerService.UpdateBannerAsync(banner);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> Delete(long bannerId)
        {
            var result = await _bannerService.GetBannerByIdAsync(bannerId);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_DeleteBanner", result.Data);
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] BannerDTO banner)
        {
            var result = await _bannerService.DeleteBannerAsync(banner.Id);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(long bannerId)
        {
            var result = await _bannerService.ChangeStatusAsync(bannerId);

            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return Json(new {success = true, message = result.Message});
        }
    }
}