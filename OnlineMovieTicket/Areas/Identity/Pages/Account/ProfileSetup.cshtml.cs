using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Enum;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.Areas.Identity.Pages.Account
{
    public class ProfileSetupModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ProfileSetupModel> _logger;
        public ProfileSetupModel(IAuthService authService, ILogger<ProfileSetupModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter your First name")]
            [StringLength(255, ErrorMessage = "First Name cannot exceed 255 characters.")]
            public string FirstName { get; set; } = string.Empty;
            [Required(ErrorMessage = "Please enter your Last name")]
            [StringLength(255, ErrorMessage = "Last Name cannot exceed 255 characters.")]
            public string LastName { get; set; } = string.Empty;
            [Required(ErrorMessage = "Please choose your gender")]
            public Gender Gender { get; set; }
            [Phone(ErrorMessage = "Invalid phone number format.")]
            public string? PhoneNumber { get; set; } 
            [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
            public string? Address { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public DateTime? DateOfBirth { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
            {
                return NotFound("User not found.");
            }
            ViewData["GenderList"] = new SelectList(Enum.GetValues(typeof(Gender)));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["GenderList"] = new SelectList(Enum.GetValues(typeof(Gender)));
                return Page();
            }

            var user = await _authService.GetUserAsync();
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var result = await _authService.UpdateUserAsync(
                user,
                Input.FirstName,
                Input.LastName,
                Input.Gender,
                Input.PhoneNumber,
                Input.Address,
                Input.DateOfBirth
            );
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to update profile.");
                ViewData["GenderList"] = new SelectList(Enum.GetValues(typeof(Gender)));
                return Page();
            }
            return Redirect("~/");
        }
    }
}