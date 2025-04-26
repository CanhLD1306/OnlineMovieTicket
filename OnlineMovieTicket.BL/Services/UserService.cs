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
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IAuthService _authService;

        public UserService(
            UserManager<ApplicationUser> userManager, 
            IAuthService authService,
            IFileUploadService fileUploadService,
            ICloudinaryService cloudinaryService,
            IUserRepository userRepository,
            ILogger<UserService> logger,
            IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _cloudinaryService = cloudinaryService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Response> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var email = await _authService.GetUserEmailAsync();
                if(!email.Success){
                    return new Response  (false, email.Message);
                }
                var user = await _authService.GetUserByEmail(email.Data!);
                if(user == null){
                    return new Response  (false, "User not found");
                }
                var token = await _authService.GeneratePasswordResetTokenAsync(user);

                var (result, updateUser) = await _authService.ResetPasswordAsync(email.Data!, token, changePasswordDTO.Password);
                if(!result.Succeeded || updateUser == null){
                    return new Response  (false, "Change password fail!");
                }
                return new Response  (true, "Change password successfull!");
            }
            catch (Exception)
            {
                return new Response (false, "Change password fail!");
            }
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

        public async Task<Response<UserDTO>> GetProfileAsync()
        {
            var email = await _authService.GetUserEmailAsync();
            if(!email.Success){
                return new Response<UserDTO>  (false, email.Message, null);
            }
            var user = await _authService.GetUserByEmail(email.Data!);
            if(user == null){
                return new Response<UserDTO>  (false, "User not found", null);
            }
            var userDTO = _mapper.Map<UserDTO>(user);
            return new Response<UserDTO> (true, null,userDTO);
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
                if(!await _authService.IsAdminAsync()){
                    return new Response(false, "You do not have permissions");
                }
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new Response (false,"User not found");
                }
                
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                return new Response (true, "User locked successfully");
            }
            catch (Exception)
            {
                return new Response (false, "User locked fail");
            }
        }

        public async Task<Response> UnlockUserAsync(string email)
        {
            try
            {
                if(!await _authService.IsAdminAsync()){
                    return new Response(false, "You do not have permissions");
                }
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return new Response (false,"User not found");
                }

                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                await _userManager.ResetAccessFailedCountAsync(user);
                return new Response (true, "User unlocked successfully");
            }
            catch (Exception)
            {
                return new Response (false, "User unlocked fail");
            }
        }

        public async Task<Response> UpdateProfileAsync(UserDTO userDTO)
        {
            try
            {
                var user = await _authService.GetUserByEmail(userDTO.Email!);
                _mapper.Map(userDTO, user);
                if (userDTO.Image != null)
                {
                    var validateAvatarResult = await _fileUploadService.ValidateImageFile(userDTO.Image);

                    if(!validateAvatarResult.Success)
                    {
                        return new Response(false, validateAvatarResult.Message);
                    }
                    var uploadResult = await _cloudinaryService.UploadProfileAsync(userDTO.Image!);
                    if (!uploadResult.Success)
                    {
                        return new Response(false, uploadResult.Message);
                    }
                    user.AvatarURL = uploadResult.Data!;
                }
                var result = await _userManager.UpdateAsync(user);
                if(!result.Succeeded){
                    return new Response(false, "Update profile fail!");
                }
                return new Response(true, "Update profile successfull!");
            }
            catch (Exception ex)
            {
                return new Response (false, ex.Message);
            }
        }
    }
}
        