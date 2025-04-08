using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<City>, int TotalCount)> GetCityAsync(
            string? searchTerm,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            IQueryable<City> query = _context.Cities.Where(c => !c.IsDeleted);
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm) || c.PostalCode.Contains(searchTerm));

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy ?? "Name"))
                : query.OrderBy(c => EF.Property<object>(c, sortBy ?? "Name"));

            int totalCount = await query.CountAsync();
            var cities = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (cities, totalCount);

        }

        public async Task<City?> GetCityByIdAsync(long id)
        {
            return await _context.Cities.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task AddCityAsync(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCityAsync(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }
    }

}