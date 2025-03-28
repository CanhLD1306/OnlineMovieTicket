using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;
using Microsoft.AspNetCore.Authentication;
using OnlineMovieTicket.DAL.Enum;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IAuthService
    {
        //
        Task<(SignInResult Result, ApplicationUser? User)> LoginAsync(string email, string password, bool rememberMe);
        Task<bool> IsAdminAsync(ApplicationUser user);
        Task<IList<AuthenticationScheme>> GetExternalAuthSchemesAsync();
        Task<(IdentityResult Result, ApplicationUser? User)> RegisterUserAsync(string email, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<bool> IsEmailConfirmationRequiredAsync();
        Task SignInUserAsync(ApplicationUser user);
        Task SignOutUserAsync();
        Task<ApplicationUser?> GetUserAsync();
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string firstName, string lastName, Gender gender, string? phoneNumber, string? address, DateTime? dateOfBirth);
        Task<ExternalLoginInfo?> GetExternalLoginInfoAsync();
        Task<(SignInResult Result, ApplicationUser? User)> ExternalLoginSignInAsync(ExternalLoginInfo info);
        Task<(IdentityResult Result, ApplicationUser? User)> RegisterExternalUserAsync(string email, ExternalLoginInfo info);
        AuthenticationProperties GetExternalAuthProperties(string provider, string redirectUrl);
        Task<(IdentityResult Result, ApplicationUser user)> ConfirmEmailAsync(string userId, string token);
        Task<(IdentityResult Result, ApplicationUser)> ResetPasswordAsync(string email, string token, string password);

    }
}