using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Models
{
    public class Showtime
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long MovieId { get; set; }
        [Required]
        public long RoomId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
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

        public Movie Movie { get; set; } = null!;

        public Room Room { get; set; } = null!;

        public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = new List<ShowtimeSeat>();
    }
}
