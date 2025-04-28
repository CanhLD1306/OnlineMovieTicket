namespace OnlineMovieTicket.BL.DTOs.Ticket
{
    public class ListTicketsDTO
    {
        public List<TicketDTO>? Tickets {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}