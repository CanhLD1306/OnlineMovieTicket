using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Models
{
    public class Room
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public long CinemaId { get; set; }
        public int Capacity { get; set; }
        [Required]
        public bool IsAvailable { get; set; }
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
        
        public Cinema Cinema { get; set; } = new Cinema();
    }
}
