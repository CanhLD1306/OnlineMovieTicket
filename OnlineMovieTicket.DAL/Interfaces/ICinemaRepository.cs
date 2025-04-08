using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ICinemaRepository
    {
        Task<(IEnumerable<Cinema>, int TotalCount)> GetCinemaAsync(
            string? searchTerm, 
            long? cityId, 
            bool? isAvailable,
            int pageNumber, 
            int pageSize, 
            string? sortBy, 
            bool isDescending);
        Task<Cinema?> GetCinemaByIdAsync(long id);
        Task AddCinemaAsync(Cinema cinema);
        Task UpdateCinemaAsync(Cinema cinema);
    }
}
