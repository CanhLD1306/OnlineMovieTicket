using System.ComponentModel.DataAnnotations;

namespace OnlineMovieTicket.BL.DTOs.Room
{
    public class RoomDTO
    {
        public long Id {get; set;}
        [Required(ErrorMessage = "Name is require")]
        [StringLength(255)]
        public string Name {get; set;} = string.Empty;
        [Required(ErrorMessage = "Please select country")]
        public long? CountryId {get; set;}
        public string? CountryName {get; set;}
        [Required(ErrorMessage = "Please select city")]
        public long? CityId {get; set;}
        public string? CityName {get; set;}
        [Required(ErrorMessage = "Please select cinema")]
        public long? CinemaId {get; set;}
        public string? CinemaName {get; set;}
        public int Capacity { get; set; } = 0;
        public bool IsAvailable { get; set; }
    }
}