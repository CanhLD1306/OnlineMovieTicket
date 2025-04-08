using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class CityService : ICityService
    {
        public Task<(IEnumerable<City>, int TotalCount)> GetCitysAsync(CityQueryDTO queryDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Response<City>> GetCityByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task AddCityAsync(City city)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCityAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCityAsync(City city)
        {
            throw new NotImplementedException();
        }
    }
}