using OnlineMovieTicket.BL.DTOs.SeatType;
using OnlineMovieTicket.BL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.BL.DTOs.City;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface ISeatTypeService
    {
        Task<IEnumerable<SeatTypeDTO>?> GetAllSeatTypesAsync();
        Task<SeatTypeList> GetAllSeatTypesAsync(SeatTypeQueryDTO queryDTO);
        Task<Response<SeatTypeDTO>> GetSeatTypeByIdAsync(long seatTypeId);
        Task<Response> UpdateSeatTypeAsync(SeatTypeDTO seatType);
    }
}
