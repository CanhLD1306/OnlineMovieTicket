using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.DTOs.Showtime;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OnlineMovieTicket.BL.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly IShowtimeSeatRepository _showtimeSeatRepository;
        private readonly IShowtimeSeatService _showtimeSeatService;
        private readonly IRoomRepository _roomRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public ShowtimeService(
            IShowtimeRepository showtimeRepository,
            IShowtimeSeatRepository showtimeSeatRepository,
            IRoomRepository roomRepository,
            IMovieRepository movieRepository,
            ISeatRepository seatRepository,
            IShowtimeSeatService showtimeSeatService,
            IMapper mapper,
            IAuthService authService)
        {
            _showtimeRepository = showtimeRepository;
            _showtimeSeatRepository = showtimeSeatRepository;
            _roomRepository = roomRepository;
            _movieRepository = movieRepository;
            _seatRepository = seatRepository;
            _showtimeSeatService = showtimeSeatService;
            _mapper = mapper;
            _authService = authService;
        }
        public async Task<Response> CreateShowtimeAsync(ShowtimeDTO showtimeDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var room = await _roomRepository.GetRoomByIdAsync(showtimeDTO.RoomId);
                    if (room == null)
                        return new Response(false, "The room does not exist");

                    var movie = await _movieRepository.GetMovieByIdAsync(showtimeDTO.MovieId);
                    if (movie == null)
                            return new Response(false, "The movie does not exist!");

                    if (movie.ReleaseDate < DateTime.Now.Date)
                    {
                        if (showtimeDTO.StartTime.Date <= DateTime.Now.Date)
                            return new Response(false, "The showtime must be at least 1 day after the current date for a movie released in the past.");
                    }
                    else if (movie.ReleaseDate > DateTime.Now.Date)
                    {
                        if (showtimeDTO.StartTime.Date < movie.ReleaseDate)
                            return new Response(false, "The showtime cannot be created before the movie's release date.");
                    }

                    if ((showtimeDTO.EndTime - showtimeDTO.StartTime).TotalMinutes < (movie.Duration + 15))
                        return new Response(false, "The selected time range is invalid. Please adjust the start and end times.");

                    if (await _showtimeRepository.IsOverLap(null, showtimeDTO.RoomId, showtimeDTO.StartTime, showtimeDTO.EndTime))
                        return new Response(false, "The selected time range overlaps with an existing showtime."); 

                    var showtime = _mapper.Map<Showtime>(showtimeDTO);
                    showtime.CreatedAt = DateTime.UtcNow;
                    showtime.CreatedBy = (await _authService.GetUserId()).Data;
                    showtime.UpdatedAt = DateTime.UtcNow;
                    showtime.UpdatedBy = (await _authService.GetUserId()).Data;
                    showtime.IsDeleted = false;

                    var result = await _showtimeRepository.CreateShowtimeAsync(showtime);

                    if (result == 0)
                    {
                        return new Response(false, "Add Showtime failed.");
                    }

                    var seats = await _seatRepository.GetAllSeatsByRoomAsync(showtimeDTO.RoomId);
                    if (seats == null || !seats.Any())
                    {
                        return new Response(false, "No seats found for the showtime.");
                    }
                    
                    var createShowtimeSeatsResult = await _showtimeSeatService.CreateShowtimeSeatsAsync(seats, result);
                    if(!createShowtimeSeatsResult.Success)
                    {
                        return new Response(false, createShowtimeSeatsResult.Message);
                    }

                    scope.Complete();
                    return new Response(true, "Add Showtime successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Add Showtime fail!");
                }
            }
        }

        public async Task<Response> DeleteShowtimeAsync(long showtimeId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var showtime = await _showtimeRepository.GetShowtimeByIdAsync(showtimeId);
                    if(showtime == null){
                        return new Response(false, "Showtime not found");
                    }

                    if (await _showtimeSeatRepository.ShowtimeHasBookedTicket(showtimeId))
                    {
                        return new Response(false, "Showtime has booked tickets, cannot delete!");
                    }

                    showtime.IsDeleted = true;
                    showtime.UpdatedAt = DateTime.UtcNow;
                    showtime.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _showtimeRepository.UpdateShowtimeAsync(showtime);

                    await _showtimeSeatService.DeleteShowtimeSeatsByRoomAsync(showtimeId);

                    scope.Complete();
                    return new Response(true, "Delete showtime successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete showtime fail!");
                }
            }
        }

        public async Task<ShowtimesList> GetShowtimesAsync(ShowtimeQueryDTO queryDTO)
        {
            var (showtimes, totalCount, filterCount) = await _showtimeRepository.GetShowtimesAsync(
                                                                        queryDTO.SearchTerm,
                                                                        queryDTO.CinemaId,
                                                                        queryDTO.RoomId,
                                                                        queryDTO.StartDate,
                                                                        queryDTO.EndDate,
                                                                        queryDTO.PageNumber,
                                                                        queryDTO.PageSize,
                                                                        queryDTO.SortBy,
                                                                        queryDTO.IsDescending
                                                                    );
            var showtimesDTO = _mapper.Map<IEnumerable<ShowtimeDTO>>(showtimes);
            return new ShowtimesList{
                Showtimes = showtimesDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<Response<ShowtimeDTO>> GetShowtimeByIdAsync(long showtimeId)
        {
            var showtime = await _showtimeRepository.GetShowtimeByIdAsync(showtimeId);
            if(showtime != null){
                var showtimeDTO = _mapper.Map<ShowtimeDTO>(showtime);
                return new Response<ShowtimeDTO>(true, null,showtimeDTO);
            }
            return new Response<ShowtimeDTO>(false,"Showtime not found");
        }

        public async Task<Response> UpdateShowtimeAsync(ShowtimeWithSeatsDTO showtimeWithSeatsDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var movie = await _movieRepository.GetMovieByIdAsync(showtimeWithSeatsDTO.Showtime.MovieId);
                    if (movie == null)
                            return new Response(false, "The movie does not exist!");
                    if (movie.ReleaseDate < DateTime.Now.Date)
                    {
                        if (showtimeWithSeatsDTO.Showtime.StartTime.Date <= DateTime.Now.Date)
                            return new Response(false, "The showtime must be at least 1 day after the current date for a movie released in the past.");
                    }
                    else if (movie.ReleaseDate > DateTime.Now.Date)
                    {
                        if (showtimeWithSeatsDTO.Showtime.StartTime.Date < movie.ReleaseDate)
                            return new Response(false, "The showtime cannot be created before the movie's release date.");
                    }
                    if (showtimeWithSeatsDTO.Showtime.StartTime <= DateTime.UtcNow)
                        return new Response(false, "Cannot update a showtime that has already started or finished.");

                    if ((showtimeWithSeatsDTO.Showtime.EndTime - showtimeWithSeatsDTO.Showtime.StartTime).TotalMinutes < (movie.Duration + 15))
                        return new Response(false, "The selected time range is invalid. Please adjust the start and end times.");

                    if (await _showtimeRepository.IsOverLap(
                    showtimeWithSeatsDTO.Showtime.Id,
                    showtimeWithSeatsDTO.Showtime.RoomId, 
                    showtimeWithSeatsDTO.Showtime.StartTime, 
                    showtimeWithSeatsDTO.Showtime.EndTime))
                        return new Response(false, "The selected time range overlaps with an existing showtime."); 

                    var showtime = await _showtimeRepository.GetShowtimeByIdAsync(showtimeWithSeatsDTO.Showtime.Id);
                    if(showtime == null){
                        return new Response(false, "Showtime not found");
                    }

                    _mapper.Map(showtimeWithSeatsDTO.Showtime, showtime);
                    showtime.UpdatedAt = DateTime.UtcNow;
                    showtime.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _showtimeRepository.UpdateShowtimeAsync(showtime);
                    scope.Complete();
                    return new Response(true, "Update showtime successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Update showtime fail!");
                }
            }
        }

        public async Task<Response<ShowtimeWithSeatsDTO>> GetShowtimeWithSeatsById(long showtimeId)
        {
            var showtime = await _showtimeRepository.GetShowtimeByIdAsync(showtimeId);
            var seats = await _showtimeSeatService.GetAllShowtimeSeatsByShowtimeAsync(showtimeId);

            if(showtime == null || seats == null || !seats.Any()){
                return new Response<ShowtimeWithSeatsDTO>(false, "Showtime or seats not found", null);
            }

            var showtimeWithSeats = new ShowtimeWithSeatsDTO {
                Showtime = _mapper.Map<ShowtimeDTO>(showtime),
                ShowtimeSeats = seats
            };

            return new Response<ShowtimeWithSeatsDTO>(true, null, showtimeWithSeats);
        }
    }
}
