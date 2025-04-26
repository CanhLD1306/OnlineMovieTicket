using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.User
{
    public class ChangePasswordDTO
        {
            [Required(ErrorMessage ="Password is require")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
}