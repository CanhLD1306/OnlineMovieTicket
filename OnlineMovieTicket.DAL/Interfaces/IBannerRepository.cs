using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface IBannerRepository
    {
        Task<IEnumerable<Banner>?> GetAllBannersAsync();
        Task<IEnumerable<Banner>?> GetBannersActiveAsync();
        Task<(IEnumerable<Banner>? banners, int totalCount, int filterCount)> GetBannerAsync(
            string? searchTerm,  
            bool? isActive,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending);
        Task<Banner?> GetBannerByIdAsync(long bannerId);
        Task CreateBannerAsync(Banner banner);
        Task UpdateBannerAsync(Banner banner);
    }
}