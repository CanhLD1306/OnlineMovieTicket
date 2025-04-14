using Microsoft.Extensions.Logging;
using OnlineMovieTicket.DAL.SeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.DAL.SeedDatas
{
    public class SeedManager
    {
        private readonly RoleSeeder _roleSeeder;
        private readonly UserSeeder _userSeeder;
        private readonly SeatTypeSeeder _seatTypeSeeder;
        private readonly ILogger<SeedManager> _logger;

        public SeedManager(
            RoleSeeder roleSeeder,
            UserSeeder userSeeder,
            SeatTypeSeeder seatTypeSeeder,
            ILogger<SeedManager> logger)
        {
            _roleSeeder = roleSeeder;
            _userSeeder = userSeeder;
            _seatTypeSeeder = seatTypeSeeder;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting seeding process...");

                await _roleSeeder.SeedRoleAsync();
                var adminUserId = await _userSeeder.SeedAdminUser();

                if (adminUserId.HasValue)
                {
                    await _seatTypeSeeder.SeedAsync(adminUserId.Value);
                }
                else
                {
                    _logger.LogWarning("Admin user ID not found. Seat types were not seeded.");
                }

                _logger.LogInformation("Seeding process completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Seeding process failed: {ex.Message}");
            }
        }
    }
}
