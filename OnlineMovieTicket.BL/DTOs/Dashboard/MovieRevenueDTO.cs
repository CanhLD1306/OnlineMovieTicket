namespace OnlineMovieTicket.BLL.DTOs.Dashboard
{
    public class MovieRevenueDTO
    {
        public string Title { get; set; } = string.Empty;
        public string PosterURL { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalTicketSold { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}