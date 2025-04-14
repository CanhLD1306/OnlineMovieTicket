using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IRoomService
    {
        Task<RoomsList> GetRoomsAsync(RoomQueryDTO queryDTO);
    }
}
