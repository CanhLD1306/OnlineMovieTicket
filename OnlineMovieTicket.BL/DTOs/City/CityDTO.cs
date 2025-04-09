using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.City
{
    public class CityDTO
    {
        public long Id {get; set;}
        [Range(1, long.MaxValue, ErrorMessage = "Please select country")]
        public long CountryId {get; set;}
        public string? CountryName {get; set;}
        [Required(ErrorMessage = "Name is require")]
        [StringLength(255)]
        public string Name {get; set;} = string.Empty;
        [Required(ErrorMessage = "PostalCode is require")]
        [StringLength(255)]
        [RegularExpression(@"^\d+$", ErrorMessage = "PostalCode must contain only numbers")]
        public string PostalCode {get; set;} = string.Empty;
        
    }
}