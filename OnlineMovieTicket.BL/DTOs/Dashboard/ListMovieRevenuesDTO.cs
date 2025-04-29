namespace OnlineMovieTicket.BLL.DTOs.Dashboard
{
    public class ListMovieRevenuesDTO
    {
        public List<MovieRevenueDTO>? MovieRevenuesDTO {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}