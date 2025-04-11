namespace OnlineMovieTicket.BL.DTOs.Cinema
{
    public class CinemasList
    {
        public IEnumerable<CinemaDTO>? Cinemas {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}