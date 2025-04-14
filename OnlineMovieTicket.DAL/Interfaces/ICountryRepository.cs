using System.Collections;
using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync();
        Task<(IEnumerable<Country> countries, int totalCount, int filterCount)> GetCountriesAsync(
            string? searchTerm, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<Country?> GetCountryByIdAsync(long countryId);
        Task<Country?> GetCountryByNameAsync(long countryId, string name);
        Task<Country?> GetCountryByCodeAsync(long countryId, string code);
        Task<long> AddCountryAsync(Country country);
        Task<long> UpdateCountryAsync(Country country);
    }
}