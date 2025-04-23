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

        public async Task<IEnumerable<City>?> GetCitiesByCountryAsync(long? countryId)
        {
            var query = _context.Cities
                                .Where(c => !c.IsDeleted)
                                .AsQueryable();
            if(countryId.HasValue && countryId > 0)
                query = query.Where(c => c.CountryId == countryId);
            
            var cities = await query.ToListAsync();
            return cities;
        }

        public async Task<(IEnumerable<City>? cities, int totalCount, int filterCount)> GetCitiesAsync(
            string? searchTerm,
            long? countryId,
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
                query = query.Where(c => c.Name.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()) 
                                || c.PostalCode.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));
            if (countryId.HasValue && countryId > 0){
                query = query.Where(c => c.CountryId == countryId);
            }

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            var filterCount = await query.CountAsync();
            var cities = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (cities, totalCount, filterCount);
        }

        public async Task<City?> GetCityByIdAsync(long cityId)
        {
            return await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId && !c.IsDeleted);
        }

        public async Task<City?> GetCityByNameAsync(long cityId, string name)
        {
            if(cityId > 0)
            {
                return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != cityId 
                                            && c.Name.Replace(" ", "").ToLower() == name.Replace(" ", "").ToLower()
                                            && !c.IsDeleted);
            }
            return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Name.Replace(" ", "").ToLower() == name.Replace(" ", "").ToLower()
                                            && !c.IsDeleted);
        }

        public async Task<City?> GetCityByPostalCodeAsync(long cityId, string postalCode)
        {
            if(cityId != 0)
            {
                return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != cityId 
                                            && c.PostalCode.Replace(" ", "").ToLower() == postalCode.Replace(" ", "").ToLower()
                                            && !c.IsDeleted);
            }
            return await _context.Cities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.PostalCode.Replace(" ", "").ToLower() == postalCode.Replace(" ", "").ToLower()
                                            && !c.IsDeleted);
        }

        public async Task CreateCityAsync(City city)
        {
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCityAsync(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CountryHasAnyCity(long countryId)
        {
            return  await _context.Cities.AnyAsync(c => c.CountryId == countryId && !c.IsDeleted);
        }
    }
}