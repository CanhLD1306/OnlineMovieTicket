using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using OnlineMovieTicket.DAL.Enum;

namespace OnlineMovieTicket.BL.DTOs.User
{
    public class UserDTO
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public string? GenderName => Gender.ToString();
        public bool IsLockedOut { get; set; }
    }
}