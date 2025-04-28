using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineMovieTicket.BL.DTOs.Ticket
{
    public class TicketDTO
    {
        public string? TicketCode {get; set;}
        public string? User {get; set;}
        public string? Movie {get; set;}
        public DateTime PurchaseDate {get; set;}
        public bool IsPaid {get; set;}
    }
}