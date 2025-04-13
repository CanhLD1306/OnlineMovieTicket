using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Models
{
    public class ShowtimeSeat
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long ShowtimeId { get; set; }
        [Required]
        public long SeatId { get; set; }
        [Required]
        public bool IsBooked { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public Guid UpdatedBy { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        public Showtime Showtime { get; set; } = new Showtime();
        public Seat Seat { get; set; } = new Seat();
    }
}
