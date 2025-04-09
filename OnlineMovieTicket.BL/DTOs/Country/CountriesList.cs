namespace OnlineMovieTicket.BL.DTOs.Country
{
    public class CountriesList
    {
        public IEnumerable<CountryDTO>? Countries {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}