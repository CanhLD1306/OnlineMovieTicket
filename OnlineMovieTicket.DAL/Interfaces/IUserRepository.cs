using OnlineMovieTicket.DAL.Enum;
using OnlineMovieTicket.DAL.Models;
namespace OnlineMovieTicket.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<(IEnumerable<ApplicationUser>? users, int totalCount, int filterCount)> GetCustomerAsync(
            string? searchTerm,
            Gender? gender,
            bool? isLocked,
            int pageNumber, 
            int pageSize);
    }
}