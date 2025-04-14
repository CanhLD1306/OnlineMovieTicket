namespace OnlineMovieTicket.BL.DTOs.SeatType
{
    public class SeatTypeList
    {
        public IEnumerable<SeatTypeDTO>? SeatTypes {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}