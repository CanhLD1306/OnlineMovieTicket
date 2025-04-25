using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.DTOs.User;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Interfaces;
using OnlineMovieTicket.DAL.Models;

namespace OnlineMovieTicket.BL.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(
            UserManager<ApplicationUser> userManager, 
            IUserRepository userRepository,
            ILogger<UserService> logger,
            IMapper mapper)
        {
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UsersList> GetCustomerAsync(UserQueryDTO queryDTO)
        {
            var (users, totalCount, filterCount) = await _userRepository.GetCustomerAsync(
                                                    queryDTO.SearchTerm,
                                                    queryDTO.Gender, 
                                                    queryDTO.IsLocked, 
                                                    queryDTO.PageNumber,
                                                    queryDTO.PageSize);

            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(users);                          
            return new UsersList
            {
                Users = userDTOs,
                TotalCount = totalCount,
                FilterCount = filterCount,
            };
        }

        public async Task<Response<UserDTO>> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new Response<UserDTO> (false,"User not found", null);
            }
            var userDTO = _mapper.Map<UserDTO>(user);   
            return new Response<UserDTO> (true, null, userDTO);
        }

        public async Task<Response> LockUserAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new Response (false,"User not found");
                }
                
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                return new Response (true, "User locked successfully");
            }
            catch (Exception ex)
            {
                return new Response (false, ex.Message);
            }
        }

        public async Task<Response> UnlockUserAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new Response (false,"User not found");
                }

                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                await _userManager.ResetAccessFailedCountAsync(user);
                return new Response (true, "User unlocked successfully");
            }
            catch (Exception ex)
            {
                return new Response (false, ex.Message);
            }
        }
    }
}
        