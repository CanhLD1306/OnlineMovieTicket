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
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Room>? rooms, int totalCount, int filterCount)> GetRoomsAsync(
            string? searchTerm, 
            long? countryId, 
            long? cityId, 
            long? cinemaId, 
            bool? isAvailable, 
            int pageNumber, 
            int pageSize, 
            string sortBy, 
            bool isDescending)
        {
            var query = _context.Rooms
                                .Include(c => c.Cinema)
                                .ThenInclude(Cinema => Cinema.City)
                                .ThenInclude(City => City.Country)
                                .Where(c => !c.IsDeleted)
                                .AsQueryable();

            var totalCount = await query.CountAsync();
        
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));
            if (countryId.HasValue && countryId > 0){
                query = query.Where(c => c.Cinema.City.Country.Id == countryId);
            }
            if (cityId.HasValue && cityId > 0){
                query = query.Where(c => c.Cinema.City.Id == cityId);
            }
            if (cinemaId.HasValue && cinemaId > 0){
                query = query.Where(c => c.CinemaId == cinemaId);
            }
            if(isAvailable.HasValue){
                query = query.Where(c => c.IsAvailable == isAvailable);
            }

            query = isDescending
                ? query.OrderByDescending(c => EF.Property<object>(c, sortBy))
                : query.OrderBy(c => EF.Property<object>(c, sortBy));

            int filterCount = await query.CountAsync();
            var rooms = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return (rooms, totalCount, filterCount);
        }

        public async Task<Room?> GetRoomByIdAsync(long roomId)
        {
            return await _context.Rooms
                                    .Include(c => c.Cinema)
                                    .ThenInclude(Cinema => Cinema.City)
                                    .ThenInclude(City => City.Country)
                                    .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);
        }

        public async Task<Room?> GetRoomByNameAsync(long CinemaId, long RoomId, string name)
        {
            if(RoomId > 0)
            {
                return await _context.Rooms
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Id != RoomId
                                            && c.CinemaId == CinemaId 
                                            && c.Name.Replace(" ", "").ToLower() == name.Replace(" ", "").ToLower()
                                            && !c.IsDeleted);
            }
            return await _context.Rooms
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(
                                            c => c.Name.Replace(" ", "").ToLower() == name.Replace(" ", "").ToLower()
                                            && c.CinemaId == CinemaId 
                                            && !c.IsDeleted);
        }
        
        public async Task<long> CreateRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room.Id;
        }

        public async Task UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Room>?> GetRoomByCinemaAsync(long cinemaId)
        {
            return await _context.Rooms.Where(c => c.CinemaId == cinemaId && !c.IsDeleted).ToListAsync();
        }

        public async Task<bool> CinemaHasRoomAsync(long cinemaId)
        {
            return await _context.Rooms.AnyAsync(c => c.CinemaId == cinemaId && !c.IsDeleted);
        }
    }
}
