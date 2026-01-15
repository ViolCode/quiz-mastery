using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [BindProperty] 
        public string Email { get; set; } = "";
        [BindProperty] 
        public string Password { get; set; } = "";
        [BindProperty]
        public bool RememberMe { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();

        private void LoadRoles()
        {
            Roles = Enum.GetValues(typeof(Role))
                .Cast<Role>()
                .Select(r => new SelectListItem
                {
                    Value = ((int)r).ToString(),
                    Text = r.ToString()
                })
                .ToList();
        }

        public IActionResult OnGet()
        {
            LoadRoles();

            var roleClaim = User.FindFirstValue(ClaimTypes.Role);

            if (!string.IsNullOrEmpty(roleClaim) && Enum.TryParse<Role>(roleClaim, out var role))
            {
                switch (role)
                {
                    case Role.Student:
                        TempData["Message"] = "You're already logged in as a student.";
                        return RedirectToPage("/Student/StudentDashboard");

                    case Role.Teacher:
                        TempData["Message"] = "You're already logged in as a teacher.";
                        return RedirectToPage("/Teacher/TeacherDashboard");
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            ModelState.Clear(); 

            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(nameof(Email), "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(nameof(Password), "Password is required.");
            }

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
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
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
