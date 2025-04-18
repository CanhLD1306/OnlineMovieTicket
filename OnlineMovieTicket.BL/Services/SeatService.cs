using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Seat;
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
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public SeatService(ISeatRepository seatRepository, IAuthService authService, IMapper mapper)
        {
            _seatRepository = seatRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SeatDTO>?> GetAllSeatsByRoomAsync(long roomId)
        {
            var seats = await _seatRepository.GetAllSeatsByRoomAsync(roomId);
            return _mapper.Map<IEnumerable<SeatDTO>>(seats);
        }
        public async Task<Response> CreateSeatsAsync(IEnumerable<SeatDTO> seatsDTO, long roomId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var seats = _mapper.Map<IEnumerable<Seat>>(seatsDTO);
                    foreach (var seat in seats)
                    {
                        seat.RoomId = roomId;
                        seat.CreatedAt = DateTime.UtcNow;
                        seat.CreatedBy = (await _authService.GetUserId()).Data;
                        seat.UpdatedAt = DateTime.UtcNow;
                        seat.UpdatedBy = (await _authService.GetUserId()).Data;
                        seat.IsDeleted = false;
                    }

                    await _seatRepository.CreateSeatsAsync(seats);
                    scope.Complete();
                    return new Response(true, "Add Seat successful!");
                }
                catch (Exception ex)
                {
                    return new Response(false, "Add Seats failed: " + ex.Message);
                }
            }
        }
        public async Task<Response> UpdateSeatsAsync(IEnumerable<SeatDTO> seatsDTO, long roomId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var updateList = new List<Seat>();
                    foreach (var seatDTO in seatsDTO)
                    {
                        var seat = await _seatRepository.GetSeatById(seatDTO.Id);
                        if(seat == null){
                            return new Response(false, "Seat not found!");
                        }
                        _mapper.Map(seatDTO, seat);
                        seat.UpdatedAt = DateTime.UtcNow;
                        seat.UpdatedBy = (await _authService.GetUserId()).Data;
                        updateList.Add(seat);
                    }

                    await _seatRepository.UpdateSeatsAsync(updateList);
                    scope.Complete();
                    return new Response(true, "Update Seat successful!");
                }
                catch (Exception ex)
                {
                    return new Response(false, "Update Seats failed: " + ex.Message);
                }
            }
        }

        public async Task<Response> DeleteSeatsByRoomAsync(long roomId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var seats = await _seatRepository.GetAllSeatsByRoomAsync(roomId);
                    if(seats == null){
                        return new Response(false, "Seats not found!");
                    }
                    foreach (var seat in seats)
                    {
                        seat.IsDeleted = true;
                        seat.UpdatedAt = DateTime.UtcNow;
                        seat.UpdatedBy = (await _authService.GetUserId()).Data;
                    }

                    await _seatRepository.UpdateSeatsAsync(seats);
                    scope.Complete();
                    return new Response(true, "Delete Seat successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete Seat fail!");
                }
            }
        }
    }
}
