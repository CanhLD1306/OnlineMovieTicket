using AutoMapper;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.SeatType;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using System.Transactions;

namespace OnlineMovieTicket.BL.Services
{
    public class SeatTypeService : ISeatTypeService
    {
        private readonly ISeatTypeRepository _seatTypeRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public SeatTypeService(
            ISeatTypeRepository seatTypeRepository,
            IAuthService authService,
            IMapper mapper)
        {
            _seatTypeRepository = seatTypeRepository;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<SeatTypeList> GetAllSeatTypesAsync(SeatTypeQueryDTO queryDTO)
        {
            var (seatTypes, totalCount, filterCount) = await _seatTypeRepository.GetAllSeatTypesAsync(
                                                                        queryDTO.SearchTerm,
                                                                        queryDTO.PageNumber,
                                                                        queryDTO.PageSize,
                                                                        queryDTO.SortBy,
                                                                        queryDTO.IsDescending
                                                                    );
            var seatTypesDTO = _mapper.Map<IEnumerable<SeatTypeDTO>>(seatTypes);

            return new SeatTypeList
            {
                SeatTypes = seatTypesDTO,
                TotalCount = totalCount,
                FilterCount = filterCount
            };
        }

        public async Task<Response<SeatTypeDTO>> GetSeatTypeByIdAsync(long seatTypeId)
        {
            var seatType = await _seatTypeRepository.GetSeatTypeByIdAsync(seatTypeId);

            if (seatType != null)
            {
                var seatTypeDTO = _mapper.Map<SeatTypeDTO>(seatType);
                return new Response<SeatTypeDTO>(true, null, seatTypeDTO);
            }
            return new Response<SeatTypeDTO>(false, "SeatType not found", null);
        }

        public async Task<Response> UpdateSeatTypeAsync(SeatTypeDTO seatTypeDTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var seatType = await _seatTypeRepository.GetSeatTypeByIdAsync(seatTypeDTO.Id);
                    if (seatType == null)
                    {
                        return new Response(false, "SeatType not found");
                    }

                    _mapper.Map(seatTypeDTO, seatType);
                    seatType.UpdatedAt = DateTime.UtcNow;
                    seatType.UpdatedBy = (await _authService.GetUserId()).Data;

                    await _seatTypeRepository.UpdateSeatTypeAsync(seatType);
                    scope.Complete();
                    return new Response(true, "Update seatType successful!");
                }
                catch (Exception ex)
                {
                    return new Response(false, "Update seatType fail " + ex.Message);
                }
            }
        }
    }
}
