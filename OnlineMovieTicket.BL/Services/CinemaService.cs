using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Cinema;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public CinemaService(ICinemaRepository cinemaRepository, IMapper mapper, IAuthService authService)
        {
            _cinemaRepository = cinemaRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CinemaDTO>?> GetCinemasByCityAsync(long? countryId)
        {
            var cinemas = await _cinemaRepository.GetCinemasByCityAsync(countryId);
            return _mapper.Map<IEnumerable<CinemaDTO>>(cinemas);
        }

        public async Task<CinemasList> GetCinemasAsync(CinemaQueryDTO queryDTO)
        {
            var(cinemas, totalCount, filterCount) = await _cinemaRepository.GetCinemaAsync(
                                                                    queryDTO.SearchTerm,
                                                                    queryDTO.CityId,
                                                                    queryDTO.CountryId,
                                                                    queryDTO.IsAvailable,
                                                                    queryDTO.PageNumber,
                                                                    queryDTO.PageSize,
                                                                    queryDTO.SortBy,
                                                                    queryDTO.IsDescending
                                                                );

            var cinemasDTO = _mapper.Map<IEnumerable<CinemaDTO>>(cinemas);
        
            return new CinemasList{
                Cinemas = cinemasDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<Response<CinemaDTO>> GetCinemaByIdAsync(long countryId)
        {
            var cinema = await _cinemaRepository.GetCinemaByIdAsync(countryId);
            if(cinema != null){
                var cinemaDTO = _mapper.Map<CinemaDTO>(cinema);
                return new Response<CinemaDTO>(true, null,cinemaDTO);
            }
            return new Response<CinemaDTO>(false,"Cinema not found");
        }

        public async Task<Response> CreateCinemaAsync(CinemaDTO cinemaDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if(await _cinemaRepository.GetCinemaByNameAsync(cinemaDTO.Id,cinemaDTO.Name) != null){
                        return new Response(false, "Cinema name already exists.");
                    }

                    var cinema = _mapper.Map<Cinema>(cinemaDTO);
                    cinema.TotalRooms = 0;
                    cinema.IsAvailable = true;
                    cinema.CreatedAt = DateTime.UtcNow;
                    cinema.CreatedBy = (await _authService.GetUserId()).Data;
                    cinema.UpdatedAt = DateTime.UtcNow;
                    cinema.UpdatedBy = (await _authService.GetUserId()).Data;
                    cinema.IsDeleted = false;

                    await _cinemaRepository.CreateCinemaAsync(cinema);
                    scope.Complete();
                    return new Response(true, "Add new cinema successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Add new cinema fail!");
                }
            }
        }
        public async Task<Response> UpdateCinemaAsync(CinemaDTO cinemaDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var cinema = await _cinemaRepository.GetCinemaByIdAsync(cinemaDTO.Id);
                    if(cinema == null){
                        return new Response(false, "Cinema not found");
                    }
                    if(await _cinemaRepository.GetCinemaByNameAsync(cinemaDTO.Id,cinemaDTO.Name) != null){
                        return new Response(false, "Cinema name already exists.");
                    }

                    _mapper.Map(cinemaDTO, cinema);
                    cinema.UpdatedAt = DateTime.UtcNow;
                    cinema.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _cinemaRepository.UpdateCinemaAsync(cinema);
                    scope.Complete();
                    return new Response(true, "Update cinema successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Update cinema fail!");
                }
            }
        }
        public async Task<Response> DeleteCinemaAsync(long countryId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var cinema = await _cinemaRepository.GetCinemaByIdAsync(countryId);
                    if(cinema == null){
                        return new Response(false, "Cinema not found");
                    }

                    if(cinema.TotalRooms > 0){
                        return new Response(false, "Cannot delete this cinema because there are still rooms associated with it.");
                    }

                    cinema.IsDeleted = true;
                    cinema.UpdatedAt = DateTime.UtcNow;
                    cinema.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _cinemaRepository.UpdateCinemaAsync(cinema);
                    scope.Complete();
                    return new Response(true, "Delete cinema successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete cinema fail!");
                }
            }
        }

        public async Task<Response> ChangeStatusAsync(long cinemaId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var cinema = await _cinemaRepository.GetCinemaByIdAsync(cinemaId);
                    if(cinema == null){
                        return new Response(false, "Cinema not found");
                    }

                    if(cinema.TotalRooms > 0){
                        return new Response(false, "Cannot change status this cinema because there are still rooms associated with it.");
                    }

                    cinema.IsAvailable = !cinema.IsAvailable;
                    cinema.UpdatedAt = DateTime.UtcNow;
                    cinema.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _cinemaRepository.UpdateCinemaAsync(cinema);
                    scope.Complete();
                    return new Response(true, "Update status successfully.");
                }
                catch (Exception)
                {
                    return new Response(false, "Update status fail!");
                }
            }
        }
    }
}