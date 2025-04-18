using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface IRoomRepository
    {
        Task<(IEnumerable<Room>? rooms, int totalCount, int filterCount)> GetRoomsAsync(
            string? searchTerm, 
            long? countryId,
            long? cityId,
            long? cinemaId,
            bool? isAvailable,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        
        Task<Room?> GetRoomByIdAsync(long CinemaId);
        Task<Room?> GetRoomByNameAsync(long CinemaId, long RoomId, string name);
        Task<long> CreateRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);

    }
}
