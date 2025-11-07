using Data.Models;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService = new UserService();

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var registeredUser = _userService.Register(user.Username, user.Password, user.Role);
            return Ok(registeredUser);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var foundUser = _userService.Login(user.Username, user.Password);
            if (foundUser == null)
                return Unauthorized("Invalid credentials");

            return Ok(foundUser);
        }
    }
}
