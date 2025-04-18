namespace OnlineMovieTicket.BL.DTOs.Movie
{
    public class MoviesList
    {
        public IEnumerable<MovieDTO>? Movies {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}