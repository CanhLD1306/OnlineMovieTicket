using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICountryRepository
    {
        Task<(IEnumerable<Country> countries, int totalCount, int filterCount)> GetCountriesAsync(
            string? searchTerm, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<Country?> GetCountryByIdAsync(long id);
        Task<Country?> GetCountryByNameAsync(long id, string name);
        Task<Country?> GetCountryByCodeAsync(long id, string code);
        Task<long> AddCountryAsync(Country country);
        Task<long> UpdateCountryAsync(Country country);
    }
}