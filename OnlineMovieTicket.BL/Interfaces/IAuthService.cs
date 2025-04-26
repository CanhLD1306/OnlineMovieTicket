using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;
using Microsoft.AspNetCore.Authentication;
using OnlineMovieTicket.DAL.Enum;
using OnlineMovieTicket.BL.DTOs;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IAuthService
    {
        // 1. Register
        Task<(IdentityResult Result, ApplicationUser? User)> RegisterUserAsync(string email, string password);
        Task<(IdentityResult Result, ApplicationUser? User)> RegisterExternalUserAsync(string email, ExternalLoginInfo info);

        // 2. Confirm email
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<(IdentityResult Result, ApplicationUser user)> ConfirmEmailAsync(string userId, string token);
        Task<bool> IsEmailConfirmationRequiredAsync();

        // 3. Login
        Task<(SignInResult Result, ApplicationUser? User)> LoginEmailAsync(string email, string password, bool rememberMe);
        Task<(SignInResult Result, ApplicationUser? User)> ExternalLoginSignInAsync(ExternalLoginInfo info);
        Task<IList<AuthenticationScheme>> GetExternalAuthSchemesAsync();
        AuthenticationProperties GetExternalAuthProperties(string provider, string redirectUrl);

        // 4. Manage login
        Task SignInUserAsync(ApplicationUser user);
        Task SignOutUserAsync();
        Task<ApplicationUser?> GetUserAsync();

        // 5. Forgot password / reset password
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<(IdentityResult Result, ApplicationUser)> ResetPasswordAsync(string email, string token, string password);

        // 6. Manage user
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<bool> IsAdminAsync(ApplicationUser user);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string firstName, string lastName, Gender gender, string? phoneNumber, string? address, DateTime? dateOfBirth);
        Task<ExternalLoginInfo?> GetExternalLoginInfoAsync();
        Task<Response<Guid>> GetUserId();
        Task<Response<String>> GetUserEmailAsync();
        Task<bool> IsAdminAsync();
    }
}