using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ICityService 
    {
        Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(long? countryId);
        Task<CitiesList> GetCitiesAsync(CityQueryDTO queryDTO);
        Task<Response<CityDTO>> GetCityByIdAsync(long cityId);
        Task<Response> CreateCityAsync(CityDTO city);
        Task<Response> UpdateCityAsync(CityDTO city);
        Task<Response> DeleteCityAsync(long cityId);
    }
}