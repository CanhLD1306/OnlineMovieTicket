using Microsoft.EntityFrameworkCore;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.Repositories
{
    public class SeatTypeRepository : ISeatTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<SeatType>? seatTypes, int totalCount, int filterCount)> GetAllSeatTypesAsync(
            string? searchTerm,
            int pageNumber,
            int pageSize,
            string sortBy,
            bool isDescending)
        {
            var query = _context.SeatTypes.Where(s => !s.IsDeleted).AsQueryable();
            var totalCount = await query.CountAsync();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.Name.Contains(searchTerm));
            }

            query = isDescending
                ? query.OrderByDescending(s => EF.Property<object>(s, sortBy))
                : query.OrderBy(s => EF.Property<object>(s, sortBy));

            var filterCount = await query.CountAsync();
            var seatTypes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (seatTypes, totalCount, filterCount);
        }

        public async Task<SeatType?> GetSeatTypeByIdAsync(long seatTypeId)
        {
            return await _context.SeatTypes.FirstOrDefaultAsync(s => s.Id == seatTypeId && !s.IsDeleted);
        }

        public async Task UpdateSeatTypeAsync(SeatType seatType)
        {
            _context.SeatTypes.Update(seatType);
            await _context.SaveChangesAsync();
        }
    }
}
