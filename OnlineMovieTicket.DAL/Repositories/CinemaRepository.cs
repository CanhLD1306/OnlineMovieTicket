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
        public async Task<(IEnumerable<Cinema> Cinemas, int TotalCount, int FilterCount)> GetCinemaAsync(
            string? searchTerm,
            long? cityId,
            long? countryId,
            bool? isAvailable,
            int pageNumber,
            int pageSize,
            string sortBy,
            bool isDescending)
        {
            var query = _context.Cinemas
                                .Include(c => c.City)
                                .ThenInclude(City => City.Country)
                                .Where(c => !c.IsDeleted)
                                .AsQueryable();

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm));
            
            if(countryId.HasValue && countryId != 0)
                query = query.Where(c => c.City.CountryId == countryId);

            if(cityId.HasValue && cityId != 0)
                query = query.Where(c => c.CityId == cityId);
            
            if(isAvailable.HasValue)
                query = query.Where(c => c.IsAvailable == isAvailable);

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            int filterCount = await query.CountAsync();

            var cinemas = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return (cinemas, totalCount, filterCount);
        }
        public async Task<Cinema?> GetCinemaByIdAsync(long id)
        {
            return await _context.Cinemas
                                    .Include(c => c.City)
                                    .ThenInclude(City => City.Country)
                                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Cinema?> GetCinemaByNameAsync(long id, string name)
        {
            if(id != 0)
            {
                return await _context.Cinemas
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != id 
                                            && c.Name == name 
                                            && !c.IsDeleted);
            }
            return await _context.Cinemas
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Name == name 
                                            && !c.IsDeleted);
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