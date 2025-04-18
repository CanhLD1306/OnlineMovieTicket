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
            long? countryId,
            long? cityId,
            long? cinemaId, 
            long? roomId,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        
        Task<Showtime?> GetShowtimeByIdAsync(long showtimeId);
        Task CreateShowtimeAsync(Showtime showtime);
        Task UpdateShowtimeAsync(Showtime showtime);
        
    }
}
