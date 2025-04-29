using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineMovieTicket.DAL.Data;
using OnlineMovieTicket.DAL.Enum;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.DAL.Interfaces
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            ApplicationDbContext context, 
            ILogger<UserRepository> logger,
            UserManager<ApplicationUser> userManager
            )
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        public async Task<(IEnumerable<ApplicationUser>? users, int totalCount, int filterCount)> GetCustomerAsync(
            string? searchTerm, 
            Gender? gender, 
            bool? isLocked, 
            int pageNumber, 
            int pageSize)
        {
            var query = (from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == "Customer"
                        select user)
                        .AsQueryable();
            
            int totalCount = await query.CountAsync();

            _logger.LogInformation("Gender: " + gender);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.Email!.Replace(" ", "").ToLower().Contains(searchTerm) ||
                    (u.FirstName + u.LastName).Replace(" ", "").ToLower().Contains(searchTerm.Replace(" ", "").ToLower()));
            }

            if (gender.HasValue)
            {
                query = query.Where(u => u.Gender == gender.Value);
            }

            if (isLocked.HasValue)
            {
                if (isLocked.Value){
                    query = query.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow);
                }
                else{
                    query = query.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value <= DateTimeOffset.UtcNow);
                }
            }
            var filterCount =  await query.CountAsync();

            var users =  await query
                                .OrderBy(u => u.FirstName)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
                                
            return (users, totalCount, filterCount);
        }

        public async Task<int> GetTotalCustomers()
        {
            return await (from user in _context.Users
                            join userRole in _context.UserRoles on user.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where role.Name == "Customer"
                            select user)
                            .CountAsync();
        }
    }
}