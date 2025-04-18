using OnlineMovieTicket.BL.DTOs.Seat;

namespace OnlineMovieTicket.BL.DTOs.Room
{
    public class RoomWithSeatsDTO
    {
        public RoomDTO Room { get; set; } = new RoomDTO();
        public IEnumerable<SeatDTO> Seats { get; set; } = new List<SeatDTO>();
    }
}
