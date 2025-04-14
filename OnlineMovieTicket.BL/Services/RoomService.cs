using AutoMapper;
using OnlineMovieTicket.BL.DTOs.City;
using OnlineMovieTicket.BL.DTOs.Room;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.BL.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public RoomService(
            IRoomRepository roomRepository,
            IAuthService authService,
            IMapper mapper)
        {
            _roomRepository = roomRepository;
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
                rooms = roomsDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }
    }
}
