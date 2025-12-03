using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PL.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";

        public IActionResult OnGet()
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (!string.IsNullOrEmpty(roleClaim))
            {
                switch (roleClaim)
                {
                    case "Student":
                        TempData["Message"] = "You're already logged in as a student.";
                        return RedirectToPage("/Student/StudentDashboard");

                    case "Teacher":
                        TempData["Message"] = "You're already logged in as a teacher.";
                        return RedirectToPage("/Teacher/TeacherDashboard");
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _userService.Login(Email, Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.RoleName) 
            };

            var identity = new ClaimsIdentity(claims, "MyCookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookie", principal);

            if (user.RoleName == "Teacher")
                return RedirectToPage("/Teacher/TeacherDashboard");
            else if (user.RoleName == "Student")
                return RedirectToPage("/Student/StudentDashboard");

            return RedirectToPage("/Auth/Register");
        }
    }
}
