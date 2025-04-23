using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Showtime
{
    public class ShowtimeQueryDTO
    {
        public int Draw {get; set;}
        [MaxLength(100)]
        public string SearchTerm { get; set; } = " ";
        public long? CinemaId { get; set; }
        public long? RoomId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; }
    }
}