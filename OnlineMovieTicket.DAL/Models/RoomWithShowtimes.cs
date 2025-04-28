namespace OnlineMovieTicket.DAL.Models
{
    public class RoomWithShowtimes
    {
        public string? Room { get; set; }
        public string? Cinema { get; set; }
        public string? Address { get; set; }
        public List<ShowtimeQueryModel> Showtimes { get; set; } = new List<ShowtimeQueryModel>();
    }

    public class ShowtimeQueryModel
    {
        public long ShowtimeId { get; set; }
        public DateTime StartTime { get; set; }
    }
}