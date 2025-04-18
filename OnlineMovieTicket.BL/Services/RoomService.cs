using AutoMapper;
using Microsoft.Extensions.Logging;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.Cinema;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.DTOs.Room;
using OnlineMovieTicket.BL.DTOs.Seat;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using OnlineMovieTicket.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OnlineMovieTicket.BL.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        private readonly ICinemaRepository _cinemaRepository;
        private readonly ISeatService _seatService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        private readonly ILogger<RoomService> _logger;

        public RoomService(
            IRoomRepository roomRepository,
            ICinemaRepository cinemaRepository,
            ILogger<RoomService> logger,
            ISeatService seatService,
            IAuthService authService,
            IMapper mapper)
        {
            _roomRepository = roomRepository;
            _cinemaRepository = cinemaRepository;
            _logger = logger;
            _seatService = seatService;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<RoomsList> GetRoomsAsync(RoomQueryDTO queryDTO)
        {
            var (rooms, totalCount, filterCount) = await _roomRepository.GetRoomsAsync(
                                                                        queryDTO.SearchTerm,
                                                                        queryDTO.CountryId,
                                                                        queryDTO.CityId,
                                                                        queryDTO.CinemaId,
                                                                        queryDTO.IsAvailable,
                                                                        queryDTO.PageNumber,
                                                                        queryDTO.PageSize,
                                                                        queryDTO.SortBy,
                                                                        queryDTO.IsDescending
                                                                    );
            var roomsDTO = _mapper.Map<IEnumerable<RoomDTO>>(rooms);

            return new RoomsList
            {
                Rooms = roomsDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<Response> CreateRoomAsync(RoomWithSeatsDTO roomWithSeats)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (roomWithSeats.Seats == null || !roomWithSeats.Seats.Any())
                    {
                        return new Response(false, "You must configure the seating arrangement before submitting.");
                    }
                    if (await _roomRepository.GetRoomByNameAsync(roomWithSeats.Room!.CinemaId, roomWithSeats.Room.Id, roomWithSeats.Room.Name) != null)
                    {
                        return new Response(false, "Room name already exists.");
                    }

                    var room = _mapper.Map<Room>(roomWithSeats.Room);
                    room.Capacity = room.Row * room.Column;
                    room.IsAvailable = true;
                    room.CreatedAt = DateTime.UtcNow;
                    room.CreatedBy = (await _authService.GetUserId()).Data;
                    room.UpdatedAt = DateTime.UtcNow;
                    room.UpdatedBy = (await _authService.GetUserId()).Data;

                    var result = await _roomRepository.CreateRoomAsync(room);

                    if (result == 0)
                    {
                        return new Response(false, "Add Room failed.");
                    }

                    var createSeatResult = await _seatService.CreateSeatsAsync(roomWithSeats.Seats, result);
                    if(!createSeatResult.Success)
                    {
                        return new Response(false, createSeatResult.Message);
                    }

                    var cinema = await _cinemaRepository.GetCinemaByIdAsync(room.CinemaId);
                    if(cinema == null){
                        return new Response(false, "Cinema not found");
                    }
                    cinema.TotalRooms = cinema.TotalRooms + 1;
                    cinema.UpdatedAt = DateTime.UtcNow;
                    cinema.UpdatedBy = (await _authService.GetUserId()).Data;
                    
                    await _cinemaRepository.UpdateCinemaAsync(cinema);

                    scope.Complete();
                    return new Response(true, "Add Room successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Add Room fail!");
                }
            }
        }

        public async Task<Response<RoomWithSeatsDTO>> GetRoomWithSeatsById(long roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if(room == null){
                _logger.LogInformation("Room is null");
            }
            var seats = await _seatService.GetAllSeatsByRoomAsync(roomId);

            if(seats == null || !seats.Any()){
                _logger.LogInformation("Seats is null");
            }

            if(room == null || seats == null || !seats.Any()){
                return new Response<RoomWithSeatsDTO>(false, "Room or seats not found", null);
            }

            var roomWithSeats = new RoomWithSeatsDTO {
                Room = _mapper.Map<RoomDTO>(room),
                Seats = seats

            };

            return new Response<RoomWithSeatsDTO>(true, null, roomWithSeats);
        }

        public async Task<Response> UpdateRoomsAsync(RoomWithSeatsDTO roomWithSeats)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (roomWithSeats.Seats == null || !roomWithSeats.Seats.Any())
                    {
                        return new Response(false, "You must configure the seating arrangement before submitting.");
                    }
                    if (await _roomRepository.GetRoomByNameAsync(roomWithSeats.Room.CinemaId, roomWithSeats.Room.Id, roomWithSeats.Room.Name) != null)
                    {
                        return new Response(false, "Room name already exists.");
                    }

                    var room = await _roomRepository.GetRoomByIdAsync(roomWithSeats.Room.Id);
                    if(room == null){
                        return new Response(false, "Room not found.");
                    }

                    _mapper.Map(roomWithSeats.Room, room);
                    room.Capacity = room.Row * room.Column;
                    room.UpdatedAt = DateTime.UtcNow;
                    room.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _roomRepository.UpdateRoomAsync(room);

                    var updateSeats = roomWithSeats.Seats.Where(seat => seat.Id != 0).ToList();

                    var newSeats = roomWithSeats.Seats.Where(seat => seat.Id == 0).ToList();

                    var updateSeatsResult = await _seatService.UpdateSeatsAsync(updateSeats, roomWithSeats.Room.Id);
                    if(!updateSeatsResult.Success)
                    {
                        return new Response(false, updateSeatsResult.Message);
                    }

                    if(newSeats != null)
                    {
                        var createSeatResult = await _seatService.CreateSeatsAsync(newSeats, roomWithSeats.Room.Id);
                        if(!createSeatResult.Success)
                        {
                            return new Response(false, createSeatResult.Message);
                        }
                    }
                    scope.Complete();
                    return new Response(true, "Update Room successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Update Room fail!");
                }
            }
        }

        public async Task<Response> DeleteRoomAsync(long roomId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var room = await _roomRepository.GetRoomByIdAsync(roomId);
                    if(room == null){
                        return new Response(false, "Room not found");
                    }

                    room.IsDeleted = true;
                    room.UpdatedAt = DateTime.UtcNow;
                    room.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _roomRepository.UpdateRoomAsync(room);

                    await _seatService.DeleteSeatsByRoomAsync(roomId);

                    var cinema = await _cinemaRepository.GetCinemaByIdAsync(room.CinemaId);
                    if(cinema == null){
                        return new Response(false, "Cinema not found");
                    }
                    cinema.TotalRooms = cinema.TotalRooms - 1;
                    cinema.UpdatedAt = DateTime.UtcNow;
                    cinema.UpdatedBy = (await _authService.GetUserId()).Data;
                    
                    await _cinemaRepository.UpdateCinemaAsync(cinema);

                    scope.Complete();
                    return new Response(true, "Delete room successful!");
                }
                catch (Exception)
                {
                    return new Response(false, "Delete room fail!");
                }
            }
        }

        public async Task<Response<RoomDTO>> GetRoomsByIdAsync(long roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);

            if(room != null){
                var roomDTO = _mapper.Map<RoomDTO>(room);
                return new Response<RoomDTO>(true, null, roomDTO);
            }
            return new Response<RoomDTO>(false,"Room not found", null);
        }

        public async Task<Response> ChangeStatusAsync(long roomId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var room = await _roomRepository.GetRoomByIdAsync(roomId);
                    if(room == null){
                        return new Response(false, "Room not found");
                    }

                    room.IsAvailable = !room.IsAvailable;
                    room.UpdatedAt = DateTime.UtcNow;
                    room.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _roomRepository.UpdateRoomAsync(room);
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
