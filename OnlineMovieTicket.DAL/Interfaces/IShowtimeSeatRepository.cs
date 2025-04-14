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
        Task<IEnumerable<ShowtimeSeat>?> GetShowtimeSeatsAsync(long ShowtimeId);
        Task AddShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeats);
        Task UpdateShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeats);
    }
}
