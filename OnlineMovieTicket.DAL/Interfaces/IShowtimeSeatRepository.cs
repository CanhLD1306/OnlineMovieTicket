using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface IShowtimeSeatRepository
    {
        Task<IEnumerable<ShowtimeSeat>?> GetShowtimeSeatsByShowtimeAsync(long ShowtimeId);
        Task<ShowtimeSeat?> GetShowtimeSeatByIdAsync(long showtimeSeatId);
        Task CreateShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeats);
        Task UpdateShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeats);
        Task UpdateShowtimeSeatAsync(ShowtimeSeat showtimeSeat);
        Task<bool> ShowtimeHasBookedTicket(long showtimeId);
    }
}
