using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OnlineMovieTicket.BL.Interfaces;
using OnlineMovieTicket.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineMovieTicket.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(SignInResult Result, ApplicationUser? User)> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (SignInResult.Failed, null);

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);
            return (result, user);
        }
        public async Task<IList<AuthenticationScheme>> GetExternalAuthSchemesAsync()
        {
            return (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<bool> IsAdminAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        
    }
}