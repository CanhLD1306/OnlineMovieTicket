using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seat>?> GetAllSeatsByRoomAsync(long roomId)
        {
            var seats = await _context.Seats
                                        .Include(s => s.SeatType)
                                        .Where(s => s.RoomId == roomId && !s.IsDeleted)
                                        .ToListAsync();
            return seats;
        }

        public async Task CreateSeatsAsync(IEnumerable<Seat> seats)
        {
            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSeatsAsync(IEnumerable<Seat> seats)
        {
            _context.Seats.UpdateRange(seats);
            await _context.SaveChangesAsync();
        }

        public async Task<Seat?> GetSeatById(long seatId)
        {
            return await _context.Seats.FirstOrDefaultAsync(c => c.Id == seatId && !c.IsDeleted);
        }
    }
}
