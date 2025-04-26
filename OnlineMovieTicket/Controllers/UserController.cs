    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineMovieTicket.BL.DTOs.Movie;
    using OnlineMovieTicket.BL.DTOs.User;
    using OnlineMovieTicket.BL.Interfaces;
    using OnlineMovieTicket.ViewModels;

    namespace OnlineMovieTicket.Controllers
    {
        [Authorize]
        public class UserController : Controller
        {
            private readonly ILogger<MovieController> _logger;
            private readonly IUserService _userService;

            public UserController(
                IUserService userService,
                ILogger<MovieController> logger)
            {
                _userService = userService;
                _logger = logger;
            }

            public IActionResult Index()
            {
                return View();
            }

            [HttpGet("GetProfile")]
            public async Task<IActionResult> GetProfile()
            {

                var result = await _userService.GetProfileAsync();
                if(!result.Success){
                    return Json(new {success = false, message = result.Message});
                }

                return PartialView ("_PersonalInformation", result.Data);
            }

            [HttpPost("UploadProfile")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> UploadProfile([FromForm] UserDTO user)
            {
                if(!ModelState.IsValid){
                    return Json(new {success = false, message = "Invalid input data."});
                }

                var result = await _userService.UpdateProfileAsync(user);
                if(!result.Success){
                    return Json(new {success = false, message = result.Message});
                }

                return Json(new {success = true, message = result.Message});
            }

            [HttpPost("ChangePassword")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDTO changePassword)
            {
                if(!ModelState.IsValid){
                    return Json(new {success = false, message = "Invalid input data."});
                }
                var result = await _userService.ChangePasswordAsync(changePassword);
                if(!result.Success){
                    return Json(new {success = false, message = result.Message});
                }

                return Json(new {success = true, message = result.Message});
            }

        }
    }