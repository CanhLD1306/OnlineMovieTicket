using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.DTOs.Showtime;

namespace OnlineMovieTicket.BL.DTOs.Room
{
    public class RoomWithShowtimesDTO
    {
        public string? Room {get; set;}
        public string? Cinema {get; set;}
        public string? Address {get; set;}
        public List<ShowtimeQueryModelDTO>? Showtimes { get; set; } = new List<ShowtimeQueryModelDTO>();
    }
    public class ShowtimeQueryModelDTO
    {
        public long ShowtimeId { get; set; }
        public DateTime StartTime { get; set; }
    }
}