using BLL.Helpers;
using Contracts.DTOs;
using Contracts.Interfaces;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PL.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        [BindProperty]
        public UserDto RegisterUser { get; set; } = new UserDto();

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<SelectListItem> Roles { get; set; } = new();

        public IActionResult OnGet()
        {
            var roles = _userService.GetAllRoles();
            Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (!string.IsNullOrEmpty(role))
            {
                if (role == "Student")
                {
                    TempData["Message"] = "You're already logged in as a student.";
                    return RedirectToPage("/Student/StudentDashboard");
                }
                else if (role == "Teacher")
                {
                    TempData["Message"] = "You're already logged in as a teacher.";
                    return RedirectToPage("/Teacher/TeacherDashboard");
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) 
            { 
                return Page(); 
            }

            if (!PasswordValidator.IsValidPassword(RegisterUser.PasswordHash))
            {
                ModelState.AddModelError("User.PasswordHash",
                    "Password must be at least 8 characters, include 1 uppercase, 1 number, and contain only letters/numbers.");
                return Page();
            }

            if (!Regex.IsMatch(RegisterUser.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("User.Email", "Please enter a valid email address.");
                return Page();
            }

            _userService.Register(RegisterUser);
            return RedirectToPage("/Auth/Login");
        }
    }
}
