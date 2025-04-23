using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OnlineMovieTicket.BL.DTOs.Showtime
{
    public class ShowtimeDTO
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Please select a movie")]
        public long MovieId { get; set; }
        public string? MovieTitle { get; set; }
        [Required(ErrorMessage = "Please select a room")]
        public long RoomId { get; set; }
        public string? RoomName { get; set; }
        [Required(ErrorMessage = "Please select a Cinema")]
        public long CinemaId { get; set; }
        public string? CinemaName { get; set; }
        [Required(ErrorMessage = "Please select a release date.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } = DateTime.Today;
        [Required(ErrorMessage = "Please select a release date.")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; } = DateTime.Today;
    }
}