using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Movie
{
    public class MovieQueryForUserDTO
    {
        public int Draw {get; set;}
        [StringLength(100)]
        public string SearchTerm { get; set; } = " ";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsCommingSoon {get; set;}
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; }
    }
}