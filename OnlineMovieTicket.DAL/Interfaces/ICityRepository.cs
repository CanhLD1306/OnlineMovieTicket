using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>?> GetCitiesByCountryAsync(long? countryId);
        Task<(IEnumerable<City>? cities, int totalCount, int filterCount)> GetCitiesAsync(
            string? searchTerm, 
            long? countryId,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<City?> GetCityByIdAsync(long cityId);
        Task<City?> GetCityByNameAsync(long cityId, string name);
        Task<City?> GetCityByPostalCodeAsync(long cityId, string postalCode);
        Task CreateCityAsync(City city);
        Task UpdateCityAsync(City city);
        bool HasAnyCity(long countryId);
    }
}
