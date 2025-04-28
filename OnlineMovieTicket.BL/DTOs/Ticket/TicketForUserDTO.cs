using CloudinaryDotNet.Actions;

namespace OnlineMovieTicket.BL.DTOs.Ticket
{
    public class TicketForUserDTO
    {
        public string? TicketCode {get; set;}
        public string? MoviePoster {get; set;}
        public string? MovieTitle {get; set;}
        public DateTime StartTime {get; set;}
        public string? Cinema {get; set;}
        public string? Room {get; set;}
        public int RowIndex {get; set;}
        public int ColIndex {get; set;}
        public decimal Price {get; set;}
    }
}