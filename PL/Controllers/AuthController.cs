using Contracts.DTOs;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PL.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _userService.Login(email, password);
            if (user != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Unauthorized();
        }

        [HttpPost]
        public IActionResult Register(UserDto user)
        {
            _userService.Register(user);
            return RedirectToAction("Login");
        }
    }

}
