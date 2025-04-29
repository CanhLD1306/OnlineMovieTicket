namespace OnlineMovieTicket.BL.DTOs.Dashboard
{
    public class OverviewStatisticsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalTicketsSold { get; set; }
        public int TotalActiveShowtimes { get; set; }
        public int TotalCustomers { get; set; }
    }
}