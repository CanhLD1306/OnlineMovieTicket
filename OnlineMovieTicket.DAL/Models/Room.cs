using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Range(1,26)]
        public int Row { get; set; }
        [Range(1, 20)]
        public int Column { get; set; }
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
        
        public Cinema Cinema { get; set; } = null!;
    }
}
