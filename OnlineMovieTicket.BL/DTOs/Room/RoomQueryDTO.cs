using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Room
{
    public class RoomQueryDTO
    {
        public int Draw {get; set;}
        [MaxLength(100)]
        public string? SearchTerm { get; set; }
        public long CountryId {get; set;}
        public long CityId {get; set;}
        public long CinemaId {get; set;}
        public bool IsAvailable {get; set;}
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "CreatedAt";
        public bool IsDescending { get; set; }
    }
}