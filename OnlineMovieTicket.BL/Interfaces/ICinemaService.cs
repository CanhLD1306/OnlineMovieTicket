using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICinemaService 
    {
        Task<(IEnumerable<Cinema>, int TotalCount)> GetCinemasAsync(CinemaQueryDTO queryDTO);
        Task<Response<Cinema>> GetCinemaByIdAsync(long id);
        Task AddCinemaAsync(Cinema cinema);
        Task UpdateCinemaAsync(Cinema cinema);
        Task DeleteCinemaAsync(long id);
    }
}