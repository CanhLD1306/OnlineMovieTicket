using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Seat
{
    public class SeatDTO
    {
        public long Id {get; set;}
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public long RoomId {get; set;}
        public long SeatTypeId {get; set;}
        public string Color {get; set;} = string.Empty;
        public bool IsDeleted { get; set; }
    }
}