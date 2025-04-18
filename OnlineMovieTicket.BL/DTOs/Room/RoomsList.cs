namespace OnlineMovieTicket.BL.DTOs.Room
{
    public class RoomsList
    {
        public IEnumerable<RoomDTO>? Rooms {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}