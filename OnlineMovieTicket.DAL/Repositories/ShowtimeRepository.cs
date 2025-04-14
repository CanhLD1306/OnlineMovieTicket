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
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Showtime?> GetShowtimeByIdAsync(long showtimeId)
        {
            return await _context.Showtime.FirstOrDefaultAsync(s => s.Id == showtimeId && !s.IsDeleted);
        }

        public async Task<(IEnumerable<Showtime>? showtimes, int totalCount, int filterCount)> GetShowtimesAsync(
            long? countryId, 
            long? cityId, 
            long? cinemaId, 
            long? roomId, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Showtime
                                .Include(c => c.Room)
                                .ThenInclude(Room => Room.Cinema)
                                .ThenInclude(Cinema => Cinema.City)
                                .ThenInclude(City => City.Country)
                                .Where(c => !c.IsDeleted)
                                .AsQueryable(); 

            var totalCount = await query.CountAsync();
            if (countryId.HasValue && countryId > 0){
                query = query.Where(c => c.Room != null &&
                                        c.Room.Cinema != null &&
                                        c.Room.Cinema.City != null &&
                                        c.Room.Cinema.City.Country != null &&
                                        c.Room.Cinema.City.Country.Id == countryId);
            }
            if (cityId.HasValue && cityId > 0){
                query = query.Where(c => c.Room != null &&
                                        c.Room.Cinema != null &&
                                        c.Room.Cinema.City != null &&
                                        c.Room.Cinema.City.Id == cityId);
            }
            if (cinemaId.HasValue && cinemaId > 0){
                query = query.Where(c => c.Room != null &&
                                        c.Room.Cinema != null &&
                                        c.Room.Cinema.Id == cinemaId);
            }
            if (roomId.HasValue && roomId > 0){
                query = query.Where(c => c.Room != null &&
                                        c.Room.Id == roomId);
            }

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            int filterCount = await query.CountAsync();
            var showtimes = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (showtimes, totalCount, filterCount);
        }

        public Task AddShowtimeAsync(Showtime showtime)
        {
            throw new NotImplementedException();
        }

        public Task UpdateShowtimeAsync(Showtime showtime)
        {
            throw new NotImplementedException();
        }
    }
}
