using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class ResetPasswordModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(IAuthService authService, ILogger<ResetPasswordModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage ="Email is require")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage ="Password is require")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;
            [Required]
            public bool IsExternalRegister {get ; set; }
        }

        public IActionResult OnGet(string? token = null, string? email = null, bool isExternalRegister = false)
        {
            if (token != null && email != null)
            {
                _logger.LogInformation("Token: " + token);
                Input = new InputModel
                {
                    Email = email,
                    IsExternalRegister = isExternalRegister,
                    Token = token
                };
                ViewData["isExternalRegister"] = isExternalRegister;
                return Page();
                
            }
            return BadRequest("A token and email must be supplied for password reset.");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var (result, user) = await _authService.ResetPasswordAsync(Input.Email, Input.Token, Input.Password);
            _logger.LogInformation("Token: " + Input.Token);
            if (result.Succeeded && user != null)
            {
                await _authService.SignInUserAsync(user);
                return RedirectToPage("/Account/ResetPasswordConfirmation", new {
                    area = "Identity", 
                    isExternalRegister = true
                });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
