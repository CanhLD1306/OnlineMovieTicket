using Microsoft.AspNetCore.Mvc;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemaRepository _cinemaRepository;

        public CinemaService(ICinemaRepository cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<(IEnumerable<Cinema>, int TotalCount)> GetCinemasAsync(CinemaQueryDTO queryDTO)
        {
            var(cinemas, totalCount) = await _cinemaRepository.GetCinemaAsync(
                                                                    queryDTO.SearchTerm,
                                                                    queryDTO.CityId,
                                                                    queryDTO.IsAvailable,
                                                                    queryDTO.PageNumber,
                                                                    queryDTO.PageSize,
                                                                    queryDTO.SortBy,
                                                                    queryDTO.IsDescending
                                                                );
            return (cinemas, totalCount);
        }

        public async Task<Response<Cinema>> GetCinemaByIdAsync(long id)
        {
            var cinema = await _cinemaRepository.GetCinemaByIdAsync(id);
            if(cinema != null){
                return new Response<Cinema>(true, null,cinema);
            }
            return new Response<Cinema>(false,"Cinema not found");
        }

        public Task AddCinemaAsync(Cinema cinema)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCinemaAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCinemaAsync(Cinema cinema)
        {
            throw new NotImplementedException();
        }
    }
}