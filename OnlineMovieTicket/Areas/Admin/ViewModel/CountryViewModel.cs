using OnlineMovieTicket.BL.DTOs.Country;

namespace OnlineMovieTicket.Areas.Admin.ViewModel
{
    public class CountryViewModel
    {
        public CountryDTO countryDTO {get; set;} = new CountryDTO();
        public CountryQueryDTO countryQueryDTO {get; set;} = new CountryQueryDTO();
    }
}