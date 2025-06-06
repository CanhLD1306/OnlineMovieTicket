﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<ShowtimeSeat>?> GetShowtimeSeatsByShowtimeAsync(long ShowtimeId)
        {
            var ShowtimeSeats = await _context.ShowtimeSeats
                                                .Include(s => s.Seat)
                                                .Include(s => s.Seat.SeatType)
                                                .Where(s => s.ShowtimeId == ShowtimeId && !s.IsDeleted)
                                                .ToListAsync();
            return ShowtimeSeats;
        }

        public async Task CreateShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeatsseats)
        {
            _context.ShowtimeSeats.AddRange(showtimeSeatsseats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShowtimeSeatsAsync(IEnumerable<ShowtimeSeat> showtimeSeats)
        {
            _context.ShowtimeSeats.UpdateRange(showtimeSeats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateShowtimeSeatAsync(ShowtimeSeat showtimeSeat)
        {
            _context.ShowtimeSeats.Update(showtimeSeat);
            await _context.SaveChangesAsync();
        }

        public async Task<ShowtimeSeat?> GetShowtimeSeatByIdAsync(long showtimeSeatId)
        {
            return await _context.ShowtimeSeats.FirstOrDefaultAsync(c => c.Id == showtimeSeatId && !c.IsDeleted);
        }

        public async Task<bool> ShowtimeHasBookedTicket(long showtimeId)
        {
            var result = await _context.ShowtimeSeats.AnyAsync(s => s.ShowtimeId == showtimeId && s.IsBooked && !s.IsDeleted);
            return result;
        }
    }
}
