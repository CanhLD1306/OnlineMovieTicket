using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace OnlineMovieTicket.BL.DTOs.Movie
{
    public class MovieDTO
    {
        public long Id {get; set;}

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        [StringLength(255)]
        public string? Author { get; set; }
        [StringLength(255)]
        public string? Actor { get; set; }
        [StringLength(255)]
        public string? Genre { get; set; }
        [Required(ErrorMessage = "Please enter the duration.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public int Duration { get; set; } = 1;
        [Required(ErrorMessage = "Please enter the price.")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public decimal Price { get; set; } = 1;
        [Required(ErrorMessage = "Please select a release date.")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow.AddDays(5);
        [StringLength(1000)]
        public string? Description { get; set; }
        public IFormFile? PosterFile { get; set; }
        [StringLength(1000)]
        public string? PosterURL { get; set; }
        public IFormFile? BannerFile { get; set; }
        [StringLength(1000)]
        public string? BannerURL { get; set; }
        [Required(ErrorMessage = "Please enter the trailer URL.")]
        [StringLength(1000)]
        public string? TrailerURL { get; set; }
    }
}