using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Movie Movie { get; set; } = new Movie();
        public Room Room { get; set; } = new Room();
    }
}
