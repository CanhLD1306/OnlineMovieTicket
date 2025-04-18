using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineMovieTicket.BL.DTOs.Movie
{
    public class MovieDTO
    {
        public long Id {get; set;}

        [Required(ErrorMessage = "Title is require")]
        [StringLength(255)]
        public string Title {get; set;} = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage = "Image is require")]
        public IFormFile? Image { get; set; }
        public bool IsActive { get; set; }
    }
}