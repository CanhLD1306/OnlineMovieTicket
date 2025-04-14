using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OnlineMovieTicket.DAL.Models;
using OnlineMovieTicket.BL.Interfaces;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordModel(IAuthService authService, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _authService = authService;
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _authService.GetUserByEmail(Input.Email);
                if (user == null || !(await _authService.IsEmailConfirmationRequiredAsync()))
                {
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
                if(await _authService.IsAdminAsync(user))
                {
                    return Page();
                }

                var token = await _authService.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { 
                        area = "Identity", 
                        email = Input.Email, 
                        token = token , 
                        isExternalRegister = false},
                    protocol: Request.Scheme)!;

                var placeholder = new Dictionary<string, string>
                    {
                        { "callbackUrl", callbackUrl }
                    };

                await _emailService.SendEmailAsync(Input.Email, "Reset Your Password","ForgotPassword",placeholder);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
