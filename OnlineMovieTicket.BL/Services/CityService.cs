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
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public CityService(
            ICityRepository cityRepository,
            ICinemaRepository cinemaRepository,
            IAuthService authService, 
            IMapper mapper
)
        {
            _cinemaRepository = cinemaRepository;
            _cityRepository = cityRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CityDTO>> GetCitiesByCountryAsync(long? countryId)
        {
            var cities = await _cityRepository.GetCitiesByCountryAsync(countryId);
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

        public async Task<Response<CityDTO>> GetCityByIdAsync(long cityId)
        {
            
            var city = await _cityRepository.GetCityByIdAsync(cityId);

            if(city != null){
                var cityDTO = _mapper.Map<CityDTO>(city);
                return new Response<CityDTO>(true, null, cityDTO);
            }
            return new Response<CityDTO>(false,"City not found", null);
        }

        public async Task<Response> CreateCityAsync(CityDTO cityDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(!await _authService.IsAdminAsync()){
                        return new Response(false, "You do not have permissions");
                    }
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

                    await _cityRepository.CreateCityAsync(city);
                    scope.Complete();
                    return new Response(true, "Add new city successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Add new city fail!");
                }
            }
        }

        public async Task<Response> UpdateCityAsync(CityDTO cityDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(!await _authService.IsAdminAsync()){
                        return new Response(false, "You do not have permissions");
                    }
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
                catch (Exception)
                {
                    return new Response(false, "Update city fail!");
                }
            }
        }

        public async Task<Response> DeleteCityAsync(long cityId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(!await _authService.IsAdminAsync()){
                        return new Response(false, "You do not have permissions");
                    }
                    var city = await _cityRepository.GetCityByIdAsync(cityId);
                    if(city == null){
                        return new Response(false, "City not found");
                    }
                    if(await _cinemaRepository.CityHasAnyCinema(cityId)){
                        return new Response(false, "Cannot delete this city because there are still cinemas associated with it.");
                    }

                    city.IsDeleted = true;
                    city.UpdatedAt = DateTime.UtcNow;
                    city.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _cityRepository.UpdateCityAsync(city);
                    scope.Complete();
                    return new Response(true, "Delete city successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete city fail!");
                }
            }
        }
    }
}

