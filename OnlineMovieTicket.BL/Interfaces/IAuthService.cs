using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnlineMovieTicket.DAL.Models;
using Microsoft.AspNetCore.Authentication;

namespace OnlineMovieTicket.BL.Interfaces
{
    public interface IAuthService
    {
        Task<(SignInResult Result, ApplicationUser? User)> LoginAsync(string email, string password, bool rememberMe);
        Task<bool> IsAdminAsync(ApplicationUser user);
        Task<IList<AuthenticationScheme>> GetExternalAuthSchemesAsync();
    }
}