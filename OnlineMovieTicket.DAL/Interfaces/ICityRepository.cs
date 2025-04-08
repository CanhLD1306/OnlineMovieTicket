using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICityRepository
    {
        Task<(IEnumerable<City>, int TotalCount)> GetCityAsync(
            string? searchTerm, 
            int pageNumber, 
            int pageSize, 
            string? sortBy, 
            bool isDescending);
        Task<City?> GetCityByIdAsync(long id);
        Task AddCityAsync(City city);
        Task UpdateCityAsync(City city);
    }
}
