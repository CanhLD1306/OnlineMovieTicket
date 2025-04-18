using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Banner;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IBannerService
    {
        Task<BannersList> GetBannersAsync(BannerQueryDTO queryDTO);
        Task<Response<BannerDTO>> GetBannerByIdAsync(long bannerId);
        Task<Response> CreateBannerAsync(BannerDTO bannerDTO);
        Task<Response> UpdateBannerAsync(BannerDTO bannerDTO);
        Task<Response> DeleteBannerAsync(long bannerId);
        Task<Response> ChangeStatusAsync(long bannerId);
    }
}