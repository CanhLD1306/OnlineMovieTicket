using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.BL.Services;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            IAuthService authService,
            IEmailService emailService,
            ILogger<RegisterModel> logger)
        {
            _authService = authService;
            _emailService = emailService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter your email.")]
            [EmailAddress(ErrorMessage = "Invalid email format.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please enter your password.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please enter your confirm password.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _authService.GetExternalAuthSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _authService.GetExternalAuthSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var (result, user) = await _authService.RegisterUserAsync(Input.Email, Input.Password);
                if (result.Succeeded && user != null)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _authService.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    var placeholder = new Dictionary<string, string>
                    {
                        { "callbackUrl", callbackUrl }
                    };

                    await _emailService.SendEmailAsync(Input.Email, "Confirm your email","ConfirmEmail",placeholder);

                    if (await _authService.IsEmailConfirmationRequiredAsync())
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _authService.SignInUserAsync(user);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}
