using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineMovieTicket.DAL.Models
{
    public class Review
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long MovieId { get; set; }
        [Range(1, 10)]
        [Precision(2, 1)]
        public decimal Rating { get; set; }
        [MaxLength(1000)]
        public string? Comment { get; set; }
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
    }
}
