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
        public long CountryId {get; set;}
        public string? CountryName {get; set;}
        [Required(ErrorMessage = "Please select city")]
        public long CityId {get; set;}
        public string? CityName {get; set;}
        [Required(ErrorMessage = "Please select cinema")]
        public long CinemaId {get; set;}
        public string? CinemaName {get; set;}
        [Required(ErrorMessage = "Please enter Row and must be between 5-26")]
        [Range(5, 26, ErrorMessage = "Please enter Row and must be between 5-26")]
        public int Row {get; set; } = 5;
        [Required(ErrorMessage = "Please enter Column and must be between 5-20")]
        [Range(5, 20, ErrorMessage = "Please enter Row and must be between 5-26")]
        public int Column { get; set; } = 5;
        public int Capacity { get; set; } = 0;
        public bool IsAvailable { get; set; }
    }
}