using BLL.DTOs;
using DAL.Models; // Only BLL references DAL
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public interface IUserService
    {
        Task<ApplicationUserDto> FindByIdAsync(int id);
        Task<bool> PasswordSignInAsync(string email, string password);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUserDto> FindByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return null;

            return new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user .LastName,
                Email = user.Email
            };
        }

        public async Task<bool> PasswordSignInAsync(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            return result.Succeeded;
        }
    }
}
