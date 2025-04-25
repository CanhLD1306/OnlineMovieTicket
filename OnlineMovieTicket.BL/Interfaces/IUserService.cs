using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.User;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IUserService
    {
        Task<UsersList> GetCustomerAsync(UserQueryDTO queryDTO);
        Task<Response<UserDTO>> GetUserAsync(string email);

        Task<Response> LockUserAsync(string email);
        Task<Response> UnlockUserAsync(string email);
        
    }
}