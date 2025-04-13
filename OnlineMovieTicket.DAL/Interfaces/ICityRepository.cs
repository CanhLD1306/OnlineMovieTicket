using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>?> GetALlCitiesByCountryAsync(long? countryId);
        Task<(IEnumerable<City>, int TotalCount, int FilterCount)> GetCitiesAsync(
            string? searchTerm, 
            long? CountryId,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<City?> GetCityByIdAsync(long id);
        Task<City?> GetCityByNameAsync(long id, string name);
        Task<City?> GetCityByPostalCodeAsync(long id, string postalCode);
        Task AddCityAsync(City city);
        Task UpdateCityAsync(City city);

        bool HasAnyCinema(long id);
    }
}
