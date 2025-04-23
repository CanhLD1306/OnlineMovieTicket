using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICinemaRepository
    {
        Task<IEnumerable<Cinema>?> GetCinemasByCityAsync(long? cityId);
        Task<(IEnumerable<Cinema>? cinemas, int totalCount, int filterCount)> GetCinemaAsync(
            string? searchTerm, 
            long? countryId,
            long? cityId, 
            bool? isAvailable,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<Cinema?> GetCinemaByIdAsync(long cinemaId);
        Task<Cinema?> GetCinemaByNameAsync(long cinemaId, string name);
        Task CreateCinemaAsync(Cinema cinema);
        Task UpdateCinemaAsync(Cinema cinema);
        Task<bool> CityHasAnyCinema(long cityId);
    }
}
