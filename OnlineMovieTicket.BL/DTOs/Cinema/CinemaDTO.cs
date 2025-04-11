using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Cinema
{
    public class CinemaDTO
    {
        public long Id {get; set;}

        [Required(ErrorMessage = "Name is require")]
        [StringLength(255)]
        public string Name {get; set;} = string.Empty;
        [Required(ErrorMessage = "Please select country")]
        public long CountryId {get; set;}
        public string? CountryName {get; set;}

        [Required(ErrorMessage = "Please select city")]
        public long CityId {get; set;}
        public string? CityName {get; set;}
        public string? Address { get; set; } 
        public int TotalRooms { get; set; }
        public bool IsAvailable { get; set; }
    }
}