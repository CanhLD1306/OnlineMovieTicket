using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

        public async Task<IEnumerable<City>?> GetALlCitiesByCountryAsync(long? countryId)
        {
            var query = _context.Cities
                                .Where(c => !c.IsDeleted)
                                .AsQueryable();
            if(countryId.HasValue && countryId != 0)
                query = query.Where(c => c.CountryId == countryId);
            
            var cities = await query.ToListAsync();
            return cities;
        }

        public async Task<(IEnumerable<City>, int TotalCount, int FilterCount)> GetCitiesAsync(
            string? searchTerm,
            long? CountryId,
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Cities
                                .Include(c => c.Country)
                                .Where(c => !c.IsDeleted)
                                .AsQueryable();

            var totalCount = await query.CountAsync();
        
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm) || c.PostalCode.Contains(searchTerm));
            if (CountryId.HasValue && CountryId != 0){
                query = query.Where(c => c.CountryId == CountryId);
            }

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            int filterCount = await query.CountAsync();
            var cities = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (cities, totalCount, filterCount);
        }

        public async Task<City?> GetCityByIdAsync(long id)
        {
            return await _context.Cities.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<City?> GetCityByNameAsync(long id, string name)
        {
            if(id != 0)
            {
                return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != id 
                                            && c.Name == name 
                                            && !c.IsDeleted);
            }
            return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Name == name 
                                            && !c.IsDeleted);
        }

        public async Task<City?> GetCityByPostalCodeAsync(long id, string postalCode)
        {
            if(id != 0)
            {
                return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != id 
                                            && c.PostalCode == postalCode 
                                            && !c.IsDeleted);
            }
            return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.PostalCode == postalCode 
                                            && !c.IsDeleted);
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

        public bool HasAnyCinema(long id)
        {
            return _context.Cinemas.Any(c => c.CityId == id && !c.IsDeleted);
        }
    }
}