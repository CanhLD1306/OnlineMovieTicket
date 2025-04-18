using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineMovieTicket.BL.DTOs.Banner
{
    public class BannerDTO
    {
        public long Id {get; set;}

        [Required(ErrorMessage = "Title is require")]
        [StringLength(255)]
        public string Title {get; set;} = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}