using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICityService 
    {
        Task<(IEnumerable<City>, int TotalCount)> GetCitysAsync(CityQueryDTO queryDTO);
        Task<Response<City>> GetCityByIdAsync(long id);
        Task AddCityAsync(City city);
        Task UpdateCityAsync(City city);
        Task DeleteCityAsync(long id);
    }
}