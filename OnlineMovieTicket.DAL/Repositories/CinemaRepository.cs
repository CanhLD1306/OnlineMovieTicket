using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class CinemaRepository : ICinemaRepository
    {
        private readonly ApplicationDbContext _context;

        public CinemaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<Cinema>, int TotalCount)> GetCinemaAsync(
            string? searchTerm,
            long? cityId,
            bool? isAvailable,
            int pageNumber,
            int pageSize,
            string? sortBy,
            bool isDescending)
        {
            IQueryable<Cinema> query = _context.Cinemas.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm));

            if (cityId.HasValue)
                query = query.Where(c => c.CityId == cityId);

            if (isAvailable.HasValue)
                query = query.Where(c => c.IsAvailable == isAvailable);

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy ?? "CreatedAt"))
                : query.OrderBy(c => EF.Property<object>(c, sortBy ?? "CreatedAt"));

            int totalCount = await query.CountAsync();
            var cinemas = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (cinemas, totalCount);
        }
        public async Task<Cinema?> GetCinemaByIdAsync(long id)
        {
            return await _context.Cinemas.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task AddCinemaAsync(Cinema cinema)
        {
            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCinemaAsync(Cinema cinema)
        {
            _context.Cinemas.Update(cinema);
            await _context.SaveChangesAsync();
        }
    }
}