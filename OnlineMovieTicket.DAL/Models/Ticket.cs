using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Models
{
    public class Ticket
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string TicketCode { get; set; } = string.Empty;
        [Required]
        public long ShowtimeSeatId { get; set; }
        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public String UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public ApplicationUser? User { get; set; } = null!;

        public ShowtimeSeat ShowtimeSeat { get; set; } = null!;
    }
}
