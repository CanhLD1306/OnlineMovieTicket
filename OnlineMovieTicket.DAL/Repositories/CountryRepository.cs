using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;

        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            var countries = await _context.Countries
                                            .Where(c => !c.IsDeleted)
                                            .ToListAsync();
            return (countries);
        }

        public async Task<(IEnumerable<Country> countries, int totalCount, int filterCount)> GetCountriesAsync(
            string? searchTerm,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Countries
                                .Where(c => !c.IsDeleted)
                                .AsQueryable();
            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm) || c.Code.Contains(searchTerm));
            
            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            var filterCount = await query.CountAsync();

            var countries = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (countries, totalCount, filterCount);
        }

        public async Task<Country?> GetCountryByIdAsync(long id)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task<Country?> GetCountryByNameAsync(long id, string name)
        {
            if(id != 0)
            {
                return await _context.Countries
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != id 
                                            && c.Name == name 
                                            && !c.IsDeleted);
            }
            return await _context.Countries
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Name == name 
                                            && !c.IsDeleted);
        }

        public async Task<Country?> GetCountryByCodeAsync(long id, string code)
        {
            if(id != 0)
            {
                return await _context.Countries
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != id 
                                            && c.Code == code 
                                            && !c.IsDeleted);
            }
            return await _context.Countries
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Code == code 
                                            && !c.IsDeleted);
        }
        public async Task<long> AddCountryAsync(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country.Id;
        }
        public async Task<long> UpdateCountryAsync(Country country)
        {
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
            return country.Id;
        }

        public bool HasAnyCity(long id)
        {
            return _context.Cities.Any(c => c.CountryId == id && !c.IsDeleted);
        }
    }

}