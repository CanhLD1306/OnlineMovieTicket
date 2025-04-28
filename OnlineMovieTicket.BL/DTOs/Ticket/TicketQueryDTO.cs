using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Ticket
{
    public class TicketQueryDTO
    {
        public int Draw {get; set;}
        [MaxLength(100)]
        public string SearchTerm { get; set; } = " ";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string SortBy { get; set; } = "PurchaseDate";
        public bool IsDescending { get; set; }
    }
}