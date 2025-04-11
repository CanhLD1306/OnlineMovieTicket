using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Cinema;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICinemaService 
    {
        Task<CinemasList> GetCinemasAsync(CinemaQueryDTO queryDTO);
        Task<Response<CinemaDTO>> GetCinemaByIdAsync(long id);
        Task<Response> AddCinemaAsync(CinemaDTO cinema);
        Task<Response> UpdateCinemaAsync(CinemaDTO cinema);
        Task<Response> DeleteCinemaAsync(long id);
        Task<Response> ChangeStatusAsync(long id);
    }
}