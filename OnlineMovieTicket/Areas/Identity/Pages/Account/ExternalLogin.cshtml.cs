using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            IAuthService authService,
            ILogger<ExternalLoginModel> logger,
            IEmailService emailService)
        {
            _authService = authService;
            _logger = logger;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ProviderDisplayName { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl })!;
            var properties = _authService.GetExternalAuthProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _authService.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var (result, user) = await _authService.ExternalLoginSignInAsync(info);
            if (result.Succeeded)
            {
                return RedirectToPage("./");
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                return RedirectToPage("./ExternalLogin", new { handler = "Confirmation", returnUrl });
            }
        }
    
        public async Task<IActionResult> OnGetConfirmationAsync(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var info = await _authService.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var email = info.Principal?.FindFirstValue(ClaimTypes.Email)!;
            var (result, user) = await _authService.RegisterExternalUserAsync(email, info);
            if (result.Succeeded && user != null)
            {
                var token = await _authService.GeneratePasswordResetTokenAsync(user);
                return RedirectToPage("ResetPassword", new 
                { 
                    email = user.Email, 
                    isExternalRegister = true,
                    returnUrl = returnUrl, 
                    token = token 
                });
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
