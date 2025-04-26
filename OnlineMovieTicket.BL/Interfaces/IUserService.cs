using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.User;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IUserService
    {
        Task<UsersList> GetCustomerAsync(UserQueryDTO queryDTO);
        Task<Response<UserDTO>> GetUserAsync(string email);
        Task<Response<UserDTO>> GetProfileAsync();
        Task<Response> LockUserAsync(string email);
        Task<Response> UnlockUserAsync(string email);
        Task<Response> UpdateProfileAsync (UserDTO userDTO);
        Task<Response> ChangePasswordAsync (ChangePasswordDTO changePasswordDTO);

    }
}