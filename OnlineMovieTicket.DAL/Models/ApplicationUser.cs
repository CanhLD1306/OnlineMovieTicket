using Microsoft.AspNetCore.Identity;
using OnlineMovieTicket.DAL.Enum;
using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(255)]
        public string? FirstName { get; set; }
        [MaxLength(255)]
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        [MaxLength(500)]
        public string? AvatarURL { get; set; }
    }
}
