using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Country
{
    public class CountryDTO
    {
        public long Id {get; set;}
        [Required(ErrorMessage = "Name is require")]
        [StringLength(255)]
        public string Name {get; set;} = string.Empty;
        [Required(ErrorMessage = "Code is require")]
        [StringLength(255)]
        public string Code {get; set;} = string.Empty;
        
    }
}