using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineMovieTicket.BL.DTOs.ShowtimeSeat
{
    public class ShowtimeSeatDTO
    {
        public long Id { get; set; }
        public long ShowtimeId { get; set; }
        public long SeatId { get; set; }
        public decimal PriceMultiplier { get; set; }
        public string? color { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public bool IsBooked { get; set; }
    }
}