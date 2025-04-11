using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICityService 
    {
        Task<IEnumerable<CityDTO>> GetAllCitiesAsync(long? id);
        Task<CitiesList> GetCitiesAsync(CityQueryDTO queryDTO);
        Task<Response<CityDTO>> GetCityByIdAsync(long id);
        Task<Response> AddCityAsync(CityDTO city);
        Task<Response> UpdateCityAsync(CityDTO city);
        Task<Response> DeleteCityAsync(long id);
    }
}