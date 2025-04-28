using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.DTOs.ShowtimeSeat;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using OnlineMovieTicket.DAL.Repositories;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OnlineMovieTicket.BL.Services
{
    public class ShowtimeSeatService : IShowtimeSeatService
    {
        private readonly IShowtimeSeatRepository _showtimeSeatRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public ShowtimeSeatService(
            IShowtimeSeatRepository showtimeSeatRepository,
            IMapper mapper,
            IAuthService authService)
        {
            _showtimeSeatRepository = showtimeSeatRepository;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<Response> CreateShowtimeSeatsAsync(IEnumerable<Seat> seats, long showtimeId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var showtimeSeats = new List<ShowtimeSeat>();
                    foreach (var seat in seats)
                    {
                        var showtimeSeat = new ShowtimeSeat
                        {
                            SeatId = seat.Id,
                            ShowtimeId = showtimeId,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = (await _authService.GetUserId()).Data,
                            UpdatedAt = DateTime.UtcNow,
                            UpdatedBy = (await _authService.GetUserId()).Data,
                            IsBooked = false,
                            IsDeleted = false
                        };
                        showtimeSeats.Add(showtimeSeat);
                    }

                    await _showtimeSeatRepository.CreateShowtimeSeatsAsync(showtimeSeats);
                    scope.Complete();
                    return new Response(true, "Add showtime seats successful!");
                }
                catch (Exception ex)
                {
                    return new Response(false, "Add showtime seats failed: " + ex.Message);
                }
            }
        }

        public async Task<Response> DeleteShowtimeSeatsByRoomAsync(long showtimeId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var showtimeSeats = await _showtimeSeatRepository.GetShowtimeSeatsByShowtimeAsync(showtimeId);
                    if (showtimeSeats == null)
                    {
                        return new Response(false, "Showtime seats not found!");
                    }
                    foreach (var showtimeSeat in showtimeSeats)
                    {
                        showtimeSeat.IsDeleted = true;
                        showtimeSeat.UpdatedAt = DateTime.UtcNow;
                        showtimeSeat.UpdatedBy = (await _authService.GetUserId()).Data;
                    }

                    await _showtimeSeatRepository.UpdateShowtimeSeatsAsync(showtimeSeats);
                    scope.Complete();
                    return new Response(true, "Delete Seat successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete Seat fail!");
                }
            }
        }

        public async Task<IEnumerable<ShowtimeSeatDTO>?> GetAllShowtimeSeatsByShowtimeAsync(long showtimeId)
        {
            var showtimeSeats = await _showtimeSeatRepository.GetShowtimeSeatsByShowtimeAsync(showtimeId);
            return _mapper.Map<IEnumerable<ShowtimeSeatDTO>>(showtimeSeats);
        }

        public async Task<Response> BookShowtimeSeatAsync(long showtimeSeatId)
        {
            try
            {
                var showtimeSeat = await _showtimeSeatRepository.GetShowtimeSeatByIdAsync(showtimeSeatId);
                if(showtimeSeat == null)
                {
                    return new Response(false, "seat not found");
                }

                if(showtimeSeat.IsBooked){
                    return new Response(false, "This seat has book");
                }
                showtimeSeat.IsBooked = true;
                showtimeSeat.UpdatedAt = DateTime.UtcNow;
                showtimeSeat.UpdatedBy = (await _authService.GetUserId()).Data;

                await _showtimeSeatRepository.UpdateShowtimeSeatAsync(showtimeSeat);
                return new Response(true, "Update seat successfull");
            }
            catch (Exception)
            {
                return new Response(false, "Update seat fail");
            }    
        }
    }
}
