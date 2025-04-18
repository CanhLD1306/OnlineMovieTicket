using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Models
{
    public class Seat
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long RoomId { get; set; }
        [Required]
        public long SeatTypeId { get; set; }
        [Required]
        public int RowIndex { get; set; }
        [Required]
        public int ColumnIndex { get; set; }
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
        
        public Room Room { get; set; } = null!;
        
        public SeatType SeatType { get; set; } = null!;
    }
}
