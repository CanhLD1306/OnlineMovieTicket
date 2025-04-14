using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.SeatType
{
    public class SeatTypeDTO
    {
        public long Id {get; set;}
        [Required(ErrorMessage = "Name is require")]
        [StringLength(255)]
        public string Name {get; set;} = string.Empty;
        [Required]
        [Precision(18, 2)]
        public decimal PriceMultiplier { get; set; }
        [Required]
        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;
    }
}