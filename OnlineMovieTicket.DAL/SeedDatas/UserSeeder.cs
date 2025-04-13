using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OnlineMovieTicket.DAL.Models;
using OnlineMovieTicket.DAL.Configurations;

namespace OnlineMovieTicket.DAL.SeedData
{
    public class UserSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserSeeder> _logger;

        public UserSeeder(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<UserSeeder> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        public async Task SeedAdminUser()
        {
            var adminConfig = _configuration.GetSection("Admin").Get<AdminConfig>();

            if (adminConfig == null)
            {
                _logger.LogError("Admin user configuration not found!");
                return;
            }

            var adminUser = await _userManager.FindByEmailAsync(adminConfig.Email);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminConfig.UserName,
                    Email = adminConfig.Email,
                    NormalizedEmail = adminConfig.Email.ToUpper(),
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(adminUser, adminConfig.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation($"Admin user '{adminConfig.Email}' created successfully!");
                }
                else
                {
                    _logger.LogError("Failed to create admin user.");
                }
            }
            else
            {
                adminUser.UserName = adminConfig.UserName;
                adminUser.Email = adminConfig.Email;
                adminUser.NormalizedEmail = adminConfig.Email.ToUpper();
                adminUser.EmailConfirmed = true;

                var updateResult = await _userManager.UpdateAsync(adminUser);
                if (updateResult.Succeeded)
                {
                    _logger.LogInformation($"Admin user '{adminConfig.Email}' updated successfully!");
                }
                else
                {
                    _logger.LogError($"Failed to update admin user: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                }
                var roles = await _userManager.GetRolesAsync(adminUser);
                if (!roles.Contains("Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation($"Admin user '{adminConfig.Email}' added to 'Admin' role.");
                }
            }
        }
    }
}
