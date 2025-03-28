using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using SQLitePCL;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(IAuthService authService, ILogger<ConfirmEmailModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string token, bool isExternalRegister)
        {
            if (userId == null || token == null)
            {
                return RedirectToPage("/Index");
            }

            var (result, user) = await _authService.ConfirmEmailAsync(userId, token);
            if(result.Succeeded && user != null){
                await _authService.SignInUserAsync(user);
                if(isExternalRegister)
                {
                    return RedirectToPage("./ResetPassword");
                }
                return RedirectToPage("./ProfileSetup");
            }
            return Page();
        }
    }
}
