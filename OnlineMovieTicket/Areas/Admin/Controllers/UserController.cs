using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs.User;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetUsers")]
        public async Task<IActionResult> GetUsers([FromForm] UserQueryDTO queryModel)
        {
            var result = await _userService.GetCustomerAsync(queryModel);
            return Json(new
            {
                draw = queryModel.Draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilterCount,
                data = result.Users
            });
        }
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(string email)
        {
            var result = await _userService.GetUserAsync(email);
            if(!result.Success){
                return Json(new {success = false, message = result.Message});
            }
            return PartialView("_LockOrUnlock", result.Data);
        }

        [HttpPost("LockOrUnlockUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockOrUnlockUser([FromForm] string email,[FromForm] bool islockedOut)
        {
            if(islockedOut){
                var result = await _userService.UnlockUserAsync(email!);
                if(!result.Success){
                    return Json(new {success = false, message = result.Message});
                }
                return Json(new {success = true, message = result.Message});
            }else{
                var result = await _userService.LockUserAsync(email!);
                if(!result.Success){
                    return Json(new {success = false, message = result.Message});
                }
                return Json(new {success = true, message = result.Message});
            }
        }
    } 
}