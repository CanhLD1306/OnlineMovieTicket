using System.Transactions;
using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public CityService(ICityRepository cityRepository, IAuthService authService, IMapper mapper
)
        {
            _cityRepository = cityRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CityDTO>> GetAllCitiesAsync(long? id)
        {
            var cities = await _cityRepository.GetALlCitiesByCountryAsync(id);
            return _mapper.Map<IEnumerable<CityDTO>>(cities);
        }

        public async Task<CitiesList> GetCitiesAsync(CityQueryDTO queryDTO)
        {
            var (cities, totalCount, filterCount) = await _cityRepository.GetCitiesAsync(
                                                                        queryDTO.SearchTerm,
                                                                        queryDTO.CountryId,
                                                                        queryDTO.PageNumber,
                                                                        queryDTO.PageSize,
                                                                        queryDTO.SortBy,
                                                                        queryDTO.IsDescending
                                                                    );
                var citiesDTO = _mapper.Map<IEnumerable<CityDTO>>(cities);

                return new CitiesList
                {
                    Cities = citiesDTO,
                    TotalCount = totalCount,
                    FilterCount = filterCount
                };
        }

        public async Task<Response<CityDTO>> GetCityByIdAsync(long id)
        {
            var city = await _cityRepository.GetCityByIdAsync(id);

            if(city != null){
                var cityDTO = _mapper.Map<CityDTO>(city);
                return new Response<CityDTO>(true, null, cityDTO);
            }
            return new Response<CityDTO>(false,"City not found", null);
        }

        public async Task<Response> AddCityAsync(CityDTO cityDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(await _cityRepository.GetCityByNameAsync(cityDTO.Id,cityDTO.Name) != null){
                        return new Response(false, "City name already exists.");
                    }
                    if(await _cityRepository.GetCityByPostalCodeAsync(cityDTO.Id,cityDTO.PostalCode) != null){
                        return new Response(false, "City PostalCode already exists.");
                    }

                    var city = _mapper.Map<City>(cityDTO);
                    city.CreatedAt = DateTime.UtcNow;
                    city.CreatedBy = (await _authService.GetUserId()).Data;
                    city.UpdatedAt = DateTime.UtcNow;
                    city.UpdatedBy = (await _authService.GetUserId()).Data;
                    city.IsDeleted = false;

                    await _cityRepository.AddCityAsync(city);
                    scope.Complete();
                    return new Response(true, "Add new city successful!");
                }
                catch (Exception ex)
                {
                    return new Response(false, "Add new city fail " + ex.Message);
                }
            }
        }

        public async Task<Response> UpdateCityAsync(CityDTO cityDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var city = await _cityRepository.GetCityByIdAsync(cityDTO.Id);
                    if(city == null){
                        return new Response(false, "City not found");
                    }
                    if(await _cityRepository.GetCityByNameAsync(cityDTO.Id,cityDTO.Name) != null){
                        return new Response(false, "City name already exists.");
                    }
                    if(await _cityRepository.GetCityByPostalCodeAsync(cityDTO.Id,cityDTO.PostalCode) != null){
                        return new Response(false, "City PostalCode already exists.");
                    }

                    _mapper.Map(cityDTO, city);
                    city.UpdatedAt = DateTime.UtcNow;
                    city.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _cityRepository.UpdateCityAsync(city);
                    scope.Complete();
                    return new Response(true, "Update city successful!");
                }
                catch (Exception ex)
                {
                    return new Response(false, "Update city fail " + ex.Message);
                }
            }
        }

        public async Task<Response> DeleteCityAsync(long id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var city = await _cityRepository.GetCityByIdAsync(id);
                    if(city == null){
                        return new Response(false, "City not found");
                    }

                    city.IsDeleted = true;
                    city.UpdatedAt = DateTime.UtcNow;
                    city.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _cityRepository.UpdateCityAsync(city);
                    scope.Complete();
                    return new Response(true, "Delete city successful!");
                }
                catch (Exception ex)
                {
                    return new Response(false, "Delete city fail " + ex.Message);
                }
            }
        }
    }
}