using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs
{
    public class CityQueryDTO
    {
        [MaxLength(100)]
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}