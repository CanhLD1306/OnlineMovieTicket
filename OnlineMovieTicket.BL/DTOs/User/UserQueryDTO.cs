using System.ComponentModel.DataAnnotations;
using OnlineMovieTicket.DAL.Enum;

namespace OnlineMovieTicket.BL.DTOs.User
{
    public class UserQueryDTO
    {
        public int Draw {get; set;}
        [MaxLength(100)]
        public string SearchTerm { get; set; } = " ";
        public Gender? Gender { get; set;}
        public bool? IsLocked { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; }
    }
}