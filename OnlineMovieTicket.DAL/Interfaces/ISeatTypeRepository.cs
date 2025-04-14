using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface ISeatTypeRepository
    {
        Task<(IEnumerable<SeatType>? seatTypes, int totalCount, int filterCount)> GetAllSeatTypesAsync(
            string? searchTerm,
            int pageNumber,
            int pageSize,
            string sortBy,
            bool isDescending);
        Task<SeatType?> GetSeatTypeByIdAsync(long seatTypeId);
        Task UpdateSeatTypeAsync(SeatType seatType);
    }
}
