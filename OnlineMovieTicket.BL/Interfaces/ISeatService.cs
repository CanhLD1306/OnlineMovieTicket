using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Seat;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDTO>?> GetAllSeatsByRoomAsync(long roomId);
        Task<Response> CreateSeatsAsync(IEnumerable<SeatDTO> seatsDTO, long roomId);
        Task<Response> UpdateSeatsAsync(IEnumerable<SeatDTO> seatsDTO, long roomId);
        Task<Response> DeleteSeatsByRoomAsync(long roomId);
    }
}
