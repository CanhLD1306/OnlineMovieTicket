
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;

namespace OnlineMovieTicket.BL.DTOs.Showtime
{
    public class ShowtimeWithSeatsDTO
    {
        public ShowtimeDTO Showtime { get; set; } = new ShowtimeDTO();
        public IEnumerable<ShowtimeSeatDTO> ShowtimeSeats { get; set; } = new List<ShowtimeSeatDTO>();
    }
}
