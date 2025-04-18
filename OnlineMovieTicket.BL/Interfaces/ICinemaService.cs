using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Cinema;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICinemaService 
    {
        Task<IEnumerable<CinemaDTO>?> GetCinemasByCityAsync(long? cityId);
        Task<CinemasList> GetCinemasAsync(CinemaQueryDTO queryDTO);
        Task<Response<CinemaDTO>> GetCinemaByIdAsync(long cinemaId);
        Task<Response> CreateCinemaAsync(CinemaDTO cinema);
        Task<Response> UpdateCinemaAsync(CinemaDTO cinema);
        Task<Response> DeleteCinemaAsync(long cinemaId);
        Task<Response> ChangeStatusAsync(long cinemaId);
    }
}