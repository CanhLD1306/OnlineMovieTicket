using System.Transactions;
using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Country;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public CountryService(
            ICountryRepository countryRepository, 
            ICityRepository cityRepository,
            IAuthService authService, 
            IMapper mapper
)
        {
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CountryDTO>> GetAllCountriesAsync()
        {
            var countries = await _countryRepository.GetAllCountriesAsync();
            return _mapper.Map<IEnumerable<CountryDTO>>(countries);
        }

        public async Task<CountriesList> GetCountriesAsync(CountryQueryDTO queryDTO)
        {
            var (countries, totalCount, filterCount) = await _countryRepository.GetCountriesAsync(
                                                                    queryDTO.SearchTerm,
                                                                    queryDTO.PageNumber,
                                                                    queryDTO.PageSize,
                                                                    queryDTO.SortBy,
                                                                    queryDTO.IsDescending
                                                                );
            var countriesDTO = _mapper.Map<IEnumerable<CountryDTO>>(countries);

            return new CountriesList
            {
                Countries = countriesDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<Response<CountryDTO>> GetCountryByIdAsync(long countryid)
        {
            var country = await _countryRepository.GetCountryByIdAsync(countryid);

            if(country != null){
                var countryDTO = _mapper.Map<CountryDTO>(country);
                return new Response<CountryDTO>(true, null, countryDTO);
            }
            return new Response<CountryDTO>(false,"Country not found", null);
        }

        public async Task<Response> CreateCountryAsync(CountryDTO countryDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(await _countryRepository.GetCountryByNameAsync(countryDTO.Id,countryDTO.Name) != null){
                        return new Response(false, "Country name already exists.");
                    }
                    if(await _countryRepository.GetCountryByCodeAsync(countryDTO.Id,countryDTO.Code) != null){
                        return new Response(false, "Country code already exists.");
                    }

                    var country = _mapper.Map<Country>(countryDTO);
                    country.Code =  country.Code.ToUpper();
                    country.CreatedAt = DateTime.UtcNow;
                    country.CreatedBy = (await _authService.GetUserId()).Data;
                    country.UpdatedAt = DateTime.UtcNow;
                    country.UpdatedBy = (await _authService.GetUserId()).Data;
                    country.IsDeleted = false;

                    await _countryRepository.CreateCountryAsync(country);
                    scope.Complete();
                    return new Response(true, "Add new country successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Add new country fail!");
                }
            }
        }
        public async Task<Response> UpdateCountryAsync(CountryDTO countryDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var country = await _countryRepository.GetCountryByIdAsync(countryDTO.Id);
                    if(country == null){
                        return new Response(false, "Country not found");
                    }
                    if(await _countryRepository.GetCountryByNameAsync(countryDTO.Id,countryDTO.Name) != null){
                        return new Response(false, "Country name already exists.");
                    }
                    if(await _countryRepository.GetCountryByCodeAsync(countryDTO.Id,countryDTO.Code) != null){
                        return new Response(false, "Country code already exists.");
                    }

                    _mapper.Map(countryDTO, country);
                    country.Code =  country.Code.ToUpper();
                    country.UpdatedAt = DateTime.UtcNow;
                    country.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _countryRepository.UpdateCountryAsync(country);
                    scope.Complete();
                    return new Response(true, "Update country successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Update country fail!");
                }
            }
        }
        public async Task<Response> DeleteCountryAsync(long countryId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var country = await _countryRepository.GetCountryByIdAsync(countryId);
                    if(country == null){
                        return new Response(false, "Country not found");
                    }
                    if(_cityRepository.HasAnyCity(countryId)){
                        return new Response(false, "Cannot delete this country because there are still cities associated with it.");
                    }

                    country.IsDeleted = true;
                    country.UpdatedAt = DateTime.UtcNow;
                    country.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _countryRepository.UpdateCountryAsync(country);
                    scope.Complete();
                    return new Response(true, "Delete country successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete country fail!");
                }
            }
        }
    }
}
