using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>?> GetAllSeatsByRoomAsync(long roomId);
        Task<Seat?> GetSeatById(long seatId);
        Task CreateSeatsAsync(IEnumerable<Seat> seats);
        Task UpdateSeatsAsync(IEnumerable<Seat> seats);
    }
}
