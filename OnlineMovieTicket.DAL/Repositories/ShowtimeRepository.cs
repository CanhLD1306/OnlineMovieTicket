using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IShowtimeRepository> _logger;

        public ShowtimeRepository(ApplicationDbContext context, ILogger<IShowtimeRepository> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Showtime?> GetShowtimeByIdAsync(long showtimeId)
        {
            return await _context.Showtime
                                        .Include(c => c.Room)
                                        .ThenInclude(Room => Room.Cinema)
                                        .Include(c => c.Movie)
                                        .FirstOrDefaultAsync(s => s.Id == showtimeId && !s.IsDeleted);
        }

        public async Task<(IEnumerable<Showtime>? showtimes, int totalCount, int filterCount)> GetShowtimesAsync(
            string? searchTerm,
            long? cinemaId, 
            long? roomId,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Showtime
                                .Include(c => c.Room)
                                .ThenInclude(Room => Room.Cinema)
                                .Include(c => c.Movie)
                                .Where(c => !c.IsDeleted)
                                .AsQueryable(); 

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Movie.Title.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));

            if (cinemaId.HasValue && cinemaId > 0){
                query = query.Where(c => c.Room != null &&
                                        c.Room.Cinema != null &&
                                        c.Room.Cinema.Id == cinemaId);
            }
            if (roomId.HasValue && roomId > 0){
                query = query.Where(c => c.Room != null &&
                                        c.Room.Id == roomId);
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartTime >= startDate.Value);
            }
            
            if (endDate.HasValue)
            {
                query = query.Where(c => c.StartTime <= endDate.Value);
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

        public async Task<long> CreateShowtimeAsync(Showtime showtime)
        {
            _context.Showtime.Add(showtime);
            await _context.SaveChangesAsync();
            return showtime.Id;
        }

        public async Task UpdateShowtimeAsync(Showtime showtime)
        {
            _context.Showtime.Update(showtime);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RoomHasFutureShotime(long roomId)
        {
            return await _context.Showtime.AnyAsync(s => s.RoomId == roomId &&
                                                    s.StartTime > DateTime.UtcNow &&
                                                   !s.IsDeleted);
        }

        public async Task<bool> MovieHasFutureShotime(long movieId)
        {
            return await _context.Showtime.AnyAsync(s => s.MovieId == movieId &&
                                                    s.StartTime > DateTime.UtcNow &&
                                                   !s.IsDeleted);
        }

        public async Task<bool> IsOverLap(long? showtimeId, long roomId, DateTime startTime, DateTime endTime)
        {
            return await _context.Showtime.Where(s => s.RoomId == roomId && !s.IsDeleted)
                                            .Where(s => showtimeId == null || s.Id != showtimeId)
                                            .AnyAsync(s => startTime < s.EndTime && endTime > s.StartTime);
        }

        public async Task<List<RoomWithShowtimes>?> GetShowtimesByCinemaAndDate(long cinemaId, long movieId, DateTime selectedDate)
        {
            var now = DateTime.Now;

            DateTime startDateTime;
            DateTime endDateTime = selectedDate.Date.AddDays(1).AddTicks(-1);

            if (selectedDate.Date == now.Date)
            {
                startDateTime = now;
            }
            else
            {
                startDateTime = selectedDate.Date;
            }

            var showtimes = await _context.Showtime
                                            .AsNoTracking()
                                            .Include(s => s.Room)
                                            .ThenInclude(Room => Room.Cinema)
                                            .Where(s =>
                                                s.Room.CinemaId == cinemaId &&
                                                s.MovieId == movieId &&
                                                s.StartTime >= startDateTime && s.StartTime <= endDateTime &&
                                                !s.IsDeleted &&
                                                !s.Room.IsDeleted &&
                                                !s.Room.Cinema.IsDeleted)
                                            .ToListAsync();


            var groupedByRoom = showtimes
                                .GroupBy(s => new {RoomName = s.Room.Name, CinemaName = s.Room.Cinema.Name, CinemaAddress = s.Room.Cinema.Address })
                                .Select(group => new RoomWithShowtimes
                                {
                                    Room = group.Key.RoomName,
                                    Cinema = group.Key.CinemaName,
                                    Address = group.Key.CinemaAddress,
                                    Showtimes = group
                                    .OrderBy(s => s.StartTime)  
                                    .Select(s => new ShowtimeQueryModel
                                    {
                                        ShowtimeId = s.Id,
                                        StartTime = s.StartTime
                                    }).ToList()
                                })
                                .ToList();

            return groupedByRoom;
        }
    }
}
