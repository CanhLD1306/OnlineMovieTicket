namespace OnlineMovieTicket.BL.DTOs.Dashboard
{
    public class TicketRatioBySeatTypeDTO
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}