using Microsoft.Extensions.Logging;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.SeedDatas
{
    public class SeatTypeSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SeatTypeSeeder> _logger;

        public SeatTypeSeeder(ApplicationDbContext context, ILogger<SeatTypeSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync(Guid userId)
        {
            if (!_context.SeatTypes.Any())
            {
                var now = DateTime.UtcNow;

                var seatTypes = new List<SeatType>
                {
                    new SeatType { Name = "Standard", PriceMultiplier = 1.0m, Color = "#28A745", CreatedAt = now, UpdatedAt = now, CreatedBy = userId, UpdatedBy = userId, IsDeleted = false },
                    new SeatType { Name = "VIP", PriceMultiplier = 1.25m, Color = "#FFD700", CreatedAt = now, UpdatedAt = now, CreatedBy = userId, UpdatedBy = userId, IsDeleted = false },
                    new SeatType { Name = "Couple", PriceMultiplier = 1.5m, Color = "#FF69B4", CreatedAt = now, UpdatedAt = now, CreatedBy = userId, UpdatedBy = userId, IsDeleted = false }
                };

                _context.SeatTypes.AddRange(seatTypes);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Seeded default seat types.");
            }
        }
    }
}
