namespace OnlineMovieTicket.BL.DTOs.City
{
    public class CitiesList
    {
        public IEnumerable<CityDTO>? Cities {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}