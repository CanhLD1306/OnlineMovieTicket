using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Models
{
    public class Movie
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(255)]
        public string? Author { get; set; }
        [MaxLength(255)]
        public string? Actor { get; set; }
        [MaxLength(255)]
        public string? Genre { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        [Precision(18, 2)]
        public decimal Price { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        [MaxLength(1000)]
        public string PosterURL { get; set; } = string.Empty;
        [Required]
        [MaxLength(1000)]
        public string BannerURL { get; set; } = string.Empty;
        [Required]
        [MaxLength(1000)]
        public string TrailerURL { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
