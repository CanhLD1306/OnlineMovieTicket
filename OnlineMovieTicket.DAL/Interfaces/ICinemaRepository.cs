using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICinemaRepository
    {
        Task<(IEnumerable<Cinema> Cinemas, int TotalCount, int FilterCount)> GetCinemaAsync(
            string? searchTerm, 
            long? CountryId,
            long? cityId, 
            bool? isAvailable,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<Cinema?> GetCinemaByIdAsync(long id);
        Task<Cinema?> GetCinemaByNameAsync(long id, string name);
        Task AddCinemaAsync(Cinema cinema);
        Task UpdateCinemaAsync(Cinema cinema);
    }
}
