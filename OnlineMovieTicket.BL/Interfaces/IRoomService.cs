using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Room;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IRoomService
    {
        Task<Response<RoomDTO>> GetRoomsByIdAsync(long roomId);
        Task<RoomsList> GetRoomsAsync(RoomQueryDTO queryDTO);
        Task<Response<RoomWithSeatsDTO>> GetRoomWithSeatsById(long roomId);
        Task<Response> CreateRoomAsync(RoomWithSeatsDTO roomWithSeats);
        Task<Response> UpdateRoomsAsync(RoomWithSeatsDTO roomWithSeats);
        Task<Response> DeleteRoomAsync(long roomId);
        Task<Response> ChangeStatusAsync(long roomId);
        Task<IEnumerable<RoomDTO>?> GetRoomByCinemaAsync(long cinemaId);
    }
}
