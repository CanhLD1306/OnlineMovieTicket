namespace OnlineMovieTicket.BL.DTOs.Ticket
{
    public class ListTicketForUser
    {
        public List<TicketForUserDTO>? Tickets {get; set;}
        public int TotalCount {get; set;}
    }
}