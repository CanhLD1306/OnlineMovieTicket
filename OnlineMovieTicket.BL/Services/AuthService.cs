using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using OnlineMovieTicket.BL.DTOs;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Enum;
using OnlineMovieTicket.DAL.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMovieTicket.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // 1. Register
        public async Task<(IdentityResult Result, ApplicationUser? User)> RegisterUserAsync(string email, string password)
        {
            var newUser = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Customer");
            }
            var user = await _userManager.FindByEmailAsync(email);

            return (result, user);
        }

        public async Task<(IdentityResult Result, ApplicationUser? User)> RegisterExternalUserAsync(string email, ExternalLoginInfo info)
        {
            var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
            }

            return (result, user);
        }

        // 2. Confirm email
        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        }

        public async Task<(IdentityResult Result, ApplicationUser user)> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            return (result, user);
        }

        public Task<bool> IsEmailConfirmationRequiredAsync()
        {
            return Task.FromResult(_userManager.Options.SignIn.RequireConfirmedAccount);
        }

        // 3. Login
        public async Task<(SignInResult Result, ApplicationUser? User)> LoginEmailAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (SignInResult.Failed, null);

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);
            return (result, user);
        }

        public async Task<(SignInResult Result, ApplicationUser? User)> ExternalLoginSignInAsync(ExternalLoginInfo info)
        {
            var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            return (result, user);
        }

        public async Task<IList<AuthenticationScheme>> GetExternalAuthSchemesAsync()
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public AuthenticationProperties GetExternalAuthProperties(string provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }

        // 4. Manage login
        public async Task SignInUserAsync(ApplicationUser user)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        public Task SignOutUserAsync()
        {
            return _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser?> GetUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null)
            {
                return await _userManager.GetUserAsync(user);
            }
            return null;
        }

        // 5. Forgot password / Reset passwrod
        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        }

        public async Task<(IdentityResult Result, ApplicationUser)> ResetPasswordAsync(string email, string token, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, password);
            return (result, user);
        }

        // 6. Manage user
        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsAdminAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string firstName, string lastName, Gender gender, string? phoneNumber, string? address, DateTime? dateOfBirth)
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Gender = gender;
            user.PhoneNumber = phoneNumber;
            user.Address = address;
            user.DateOfBirth = dateOfBirth ?? DateTime.MinValue;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<ExternalLoginInfo?> GetExternalLoginInfoAsync()
        {
            return await _signInManager.GetExternalLoginInfoAsync();
        }

        public async Task<Response<Guid>> GetUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                return new Response<Guid>(false, "Unauthorized", Guid.Empty);
            }

            var user = await _userManager.FindByIdAsync(userIdString);

            if (user == null)
            {
                return new Response<Guid>(false, "User not found", Guid.Empty);
            }
            return new Response<Guid>(true, null, userId);
        }
    }
}
