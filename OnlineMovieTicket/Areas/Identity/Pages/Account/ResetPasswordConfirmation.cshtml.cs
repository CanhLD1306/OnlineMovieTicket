using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ResetPasswordConfirmationModel> _logger;

        public ResetPasswordConfirmationModel(
            IAuthService authService,
            ILogger<ResetPasswordConfirmationModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet(bool isExternalRegister = false)
        {
            if(isExternalRegister)
            {
                return RedirectToPage("ProfileSetup");
            }
            await _authService.SignOutUserAsync();
            return Page();
        }
    }
}
