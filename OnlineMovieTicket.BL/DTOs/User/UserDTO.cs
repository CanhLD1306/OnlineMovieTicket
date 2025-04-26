using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using OnlineMovieTicket.DAL.Enum;

namespace OnlineMovieTicket.BL.DTOs.User
{
    public class UserDTO
    {
        [StringLength(255)]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        [StringLength(255)]
        public string? FirstName { get; set; }
        [StringLength(255)]
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        public IFormFile? Image { get; set; }
        public string? AvatarURL { get; set; }
        public Gender Gender { get; set; }
        public string? GenderName => Gender.ToString();
        public bool IsLockedOut { get; set; }
    }
}