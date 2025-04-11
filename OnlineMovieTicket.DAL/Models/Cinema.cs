using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Models
{
    public class Cinema
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Address { get; set; }
        public int TotalRooms { get; set; }
        [Required]
        [ForeignKey(nameof(City))]
        public long CityId { get; set; }
        public bool IsAvailable { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public City? City { get; set; }
    }
}
