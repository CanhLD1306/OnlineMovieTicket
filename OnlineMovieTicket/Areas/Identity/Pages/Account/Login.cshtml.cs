using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.BL.DTOs;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IAuthService authService, ILogger<LoginModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter your email.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please enter your password.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _authService.GetExternalAuthSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _authService.GetExternalAuthSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var (result, user) = await _authService.LoginEmailAsync(Input.Email, Input.Password, Input.RememberMe);
                if (result.Succeeded && user != null)
                {
                    var redirectUrl = await _authService.IsAdminAsync(user) 
                    ? Url.Action("Index", "Dashboard", new { area = "Admin" }, protocol: Request.Scheme)
                    : returnUrl;
                    return new JsonResult(new Response<string>(true, "Login successful!", redirectUrl));
                }
                if (result.IsLockedOut)
                {
                    return new JsonResult(new Response<string>(false, "Your account is lockout. Please try again!"));
                }
                else
                {
                    return new JsonResult(new Response<string>(false, "Invalid email or password!"));
                }
            }
            return new JsonResult(new Response<string>(false, "Invalid email or password!"));
        }
    }
}
