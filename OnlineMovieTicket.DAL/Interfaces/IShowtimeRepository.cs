using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface IShowtimeRepository
    {
        Task<(IEnumerable<Showtime>? showtimes, int totalCount, int filterCount)> GetShowtimesAsync(
            string? searchTerm,
            long? cinemaId, 
            long? roomId,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        
        Task<Showtime?> GetShowtimeByIdAsync(long showtimeId);
        Task<long> CreateShowtimeAsync(Showtime showtime);
        Task UpdateShowtimeAsync(Showtime showtime);
        Task<bool> IsOverLap(long? showtimeId, long roomId, DateTime startTime, DateTime endTime);
        Task<bool> RoomHasFutureShotime(long roomId);
        Task<bool> MovieHasFutureShotime(long movieId);
        Task<List<RoomWithShowtimes>?> GetShowtimesByCinemaAndDate(long cinemaId, long movieId, DateTime selectedDate);
        Task<int> GetTotalShowtimes();
    }
}
