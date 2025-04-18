using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class BannerRepository : IBannerRepository
    {
        private readonly ApplicationDbContext _context;

        public BannerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateBannerAsync(Banner banner)
        {
            _context.Add(banner);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Banner>?> GetAllBannersAsync()
        {
            return await _context.Banners.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<(IEnumerable<Banner>? banners, int totalCount, int filterCount)> GetBannerAsync(
            string? searchTerm, 
            bool? isActive, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Banners
                                .Where(c => !c.IsDeleted)
                                .AsQueryable();
            
            int totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Title.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));

            if(isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive);

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));
            
            int filterCount = await query.CountAsync();

            var banner = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            
            return (banner, totalCount, filterCount);
        }

        public async Task<Banner?> GetBannerByIdAsync(long bannerId)
        {
            return await _context.Banners.FirstOrDefaultAsync(c => c.Id == bannerId && !c.IsDeleted);
        }

        public async Task<IEnumerable<Banner>?> GetBannersActiveAsync()
        {
            return await _context.Banners.Where(c => !c.IsDeleted && c.IsActive).ToListAsync();
        }

        public async Task UpdateBannerAsync(Banner banner)
        {
            _context.Update(banner);
            await _context.SaveChangesAsync();
        }
    }
}