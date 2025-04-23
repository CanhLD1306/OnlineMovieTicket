namespace OnlineMovieTicket.BL.DTOs.Showtime
{
    public class ShowtimesList
    {
        public IEnumerable<ShowtimeDTO>? Showtimes {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}