using System.Transactions;
using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Banner;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public BannerService(
            IBannerRepository bannerRepository,
            ICloudinaryService cloudinaryService,
            IMapper mapper, 
            IAuthService authService)
        {
            _bannerRepository = bannerRepository;
            _cloudinaryService = cloudinaryService;
            _authService = authService;
            _mapper = mapper;
        }
        public async Task<Response> ChangeStatusAsync(long bannerId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var banner = await _bannerRepository.GetBannerByIdAsync(bannerId);
                    
                    if(banner == null){
                        return new Response(false, "Banner not found");
                    }

                    if (banner.IsActive)
                    {
                        var activeBanners = await _bannerRepository.GetBannersActiveAsync();
                        
                        if (activeBanners!.Count() == 1)
                        {
                            return new Response(false, "At least one banner must remain active.");
                        }
                    }

                    banner.IsActive = !banner.IsActive;
                    banner.UpdatedAt = DateTime.UtcNow;
                    banner.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _bannerRepository.UpdateBannerAsync(banner);
                    scope.Complete();
                    return new Response(true, "Update status successfully.");
                }
                catch (Exception)
                {
                    return new Response(false, "Update status fail!");
                }
            }
        }

        public async Task<Response> CreateBannerAsync(BannerDTO bannerDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (bannerDTO.Image == null)
                    {
                        return new Response(false, "Please upload image!");
                    }
                    var banners = await _bannerRepository.GetAllBannersAsync();
                    if (banners != null && banners.Count() >= 5)
                    {
                        return new Response(false, "Cannot add more than 5 banners!");
                    }

                    var result = await _cloudinaryService.UploadBannerAsync(bannerDTO.Image);
                    if (!result.Success)
                    {
                        return new Response(false, result.Message);
                    }
                    var banner = _mapper.Map<Banner>(bannerDTO);
                    banner.ImageURL = result.Data!;
                    banner.IsActive = false;
                    banner.CreatedAt = DateTime.UtcNow;
                    banner.CreatedBy = (await _authService.GetUserId()).Data;
                    banner.UpdatedAt = DateTime.UtcNow;
                    banner.UpdatedBy = (await _authService.GetUserId()).Data;
                    banner.IsDeleted = false;

                    await _bannerRepository.CreateBannerAsync(banner);
                    scope.Complete();
                    return new Response(true, "Add new banner successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Add new banner fail!");
                }
            }
        }

        public async Task<Response> DeleteBannerAsync(long bannerId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var banner = await _bannerRepository.GetBannerByIdAsync(bannerId);
                    if(banner == null){
                        return new Response(false, "Banner not found");
                    }
                    
                    banner.IsDeleted = true;
                    banner.UpdatedAt = DateTime.UtcNow;
                    banner.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _bannerRepository.UpdateBannerAsync(banner);
                    scope.Complete();
                    return new Response(true, "Delete banner successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete banner fail!");
                }
            }
        }

        public async Task<Response<BannerDTO>> GetBannerByIdAsync(long bannerId)
        {
            var banner = await _bannerRepository.GetBannerByIdAsync(bannerId);
            if(banner != null){
                var bannerDTO = _mapper.Map<BannerDTO>(banner);
                return new Response<BannerDTO>(true, null,bannerDTO);
            }
            return new Response<BannerDTO>(false,"Banner not found");
        }

        public async Task<BannersList> GetBannersAsync(BannerQueryDTO queryDTO)
        {
            var(banners, totalCount, filterCount) = await _bannerRepository.GetBannerAsync(
                                                                    queryDTO.SearchTerm,
                                                                    queryDTO.IsActive,
                                                                    queryDTO.PageNumber,
                                                                    queryDTO.PageSize,
                                                                    queryDTO.SortBy,
                                                                    queryDTO.IsDescending
                                                                );

            var bannersDTO = _mapper.Map<IEnumerable<BannerDTO>>(banners);
        
            return new BannersList{
                Banners = bannersDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<Response> UpdateBannerAsync(BannerDTO bannerDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var banner = await _bannerRepository.GetBannerByIdAsync(bannerDTO.Id);
                    if(banner == null){
                        return new Response(false, "Banner not found");
                    }

                    _mapper.Map(bannerDTO, banner);

                    if (bannerDTO.Image != null)
                    {
                        var result = await _cloudinaryService.UploadBannerAsync(bannerDTO.Image!);
                        if (!result.Success)
                        {
                            return new Response(false, result.Message);
                        }
                        banner.ImageURL = result.Data!;
                    }

                    banner.UpdatedAt = DateTime.UtcNow;
                    banner.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _bannerRepository.UpdateBannerAsync(banner);
                    scope.Complete();
                    return new Response(true, "Update banner successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Update banner fail!");
                }
            }
        }
    }
}