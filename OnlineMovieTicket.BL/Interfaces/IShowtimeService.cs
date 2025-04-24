using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Showtime;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IShowtimeService
    {
        Task<Response> CreateShowtimeAsync(ShowtimeDTO showtimeDTO);
        Task<Response> UpdateShowtimeAsync(ShowtimeWithSeatsDTO showtimeWithSeats);
        Task<Response> DeleteShowtimeAsync(long id);
        Task<ShowtimesList> GetShowtimesAsync(ShowtimeQueryDTO queryDTO);
        Task<Response<ShowtimeDTO>> GetShowtimeByIdAsync(long showtimeId);
        Task<Response<ShowtimeWithSeatsDTO>> GetShowtimeWithSeatsById(long showtimeId);
    }
}
