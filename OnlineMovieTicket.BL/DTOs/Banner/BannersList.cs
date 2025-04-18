namespace OnlineMovieTicket.BL.DTOs.Banner
{
    public class BannersList
    {
        public IEnumerable<BannerDTO>? Banners {get; set;}
        public int TotalCount {get; set;}
        public int FilterCount {get; set;}
    }
}