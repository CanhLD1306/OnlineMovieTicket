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
    public class ShowtimeSeatRepository : IShowtimeSeatRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeSeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShowtimeSeat>?> GetShowtimeSeatsAsync(long ShowtimeId)
        {
            var ShowtimeSeats = await _context.ShowtimeSeats
                                                .Where(s => s.ShowtimeId == ShowtimeId && !s.IsDeleted)
                                                .ToListAsync();
            return ShowtimeSeats;
        }

        public async Task AddShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeatsseats)
        {
            _context.ShowtimeSeats.AddRange(showtimeSeatsseats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeats)
        {
            _context.ShowtimeSeats.AddRange(showtimeSeats);
            await _context.SaveChangesAsync();
        }
    }
}
