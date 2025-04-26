using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using OnlineMovieTicket.DAL.Enum;

namespace OnlineMovieTicket.BL.DTOs.User
{
    public class UserDTO
    {
        public string Email { get; set; } = string.Empty;
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter your First name")]
        [StringLength(255, ErrorMessage = "First Name cannot exceed 255 characters.")]
        public string? FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please enter your Last name")]
        [StringLength(255, ErrorMessage = "Last Name cannot exceed 255 characters.")]
        public string? LastName { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }
        public IFormFile? Image { get; set; }
        public string? AvatarURL { get; set; }
        [Required(ErrorMessage = "Please choose your gender")]
        public Gender Gender { get; set; }
        public string? GenderName => Gender.ToString();
        public bool IsLockedOut { get; set; }
    }
}