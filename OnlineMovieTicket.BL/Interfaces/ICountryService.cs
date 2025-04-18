using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Country;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICountryService 
    {
        Task<IEnumerable<CountryDTO>> GetAllCountriesAsync();
        Task<CountriesList> GetCountriesAsync(CountryQueryDTO queryDTO);
        Task<Response<CountryDTO>> GetCountryByIdAsync(long countryid);
        Task<Response> CreateCountryAsync(CountryDTO countryDTO);
        Task<Response> UpdateCountryAsync(CountryDTO countryDTO);
        Task<Response> DeleteCountryAsync(long countryid);
    }
}