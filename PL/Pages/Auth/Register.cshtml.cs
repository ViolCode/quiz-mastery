using BLL.Helpers;
using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
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

        [BindProperty]
        public string ConfirmPassword { get; set; } = "";

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

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

        public IActionResult OnPost()
        {
            LoadRoles();
            ModelState.Clear(); // remove default messages so we can provide friendly ones

            var hasError = false;

            if (string.IsNullOrWhiteSpace(RegisterUser.Name))
            {
                ModelState.AddModelError("RegisterUser.Name", "Name is required.");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(RegisterUser.Email))
            {
                ModelState.AddModelError("RegisterUser.Email", "Email is required.");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(RegisterUser.PasswordHash))
            {
                ModelState.AddModelError("RegisterUser.PasswordHash", "Password is required.");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPassword", "Please confirm your password.");
                hasError = true;
            }

            if (!string.IsNullOrWhiteSpace(RegisterUser.PasswordHash) &&
                !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                RegisterUser.PasswordHash != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                hasError = true;
            }

            if (!string.IsNullOrWhiteSpace(RegisterUser.PasswordHash) &&
                !PasswordValidator.IsValidPassword(RegisterUser.PasswordHash))
            {
                ModelState.AddModelError("RegisterUser.PasswordHash",
                    "Password must be at least 8 characters, include 1 uppercase, 1 number, and contain only letters/numbers.");
                hasError = true;
            }

            if (!string.IsNullOrWhiteSpace(RegisterUser.Email) &&
                !Regex.IsMatch(RegisterUser.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("RegisterUser.Email", "Please enter a valid email address.");
                hasError = true;
            }

            if (hasError)
            {
                return Page();
            }

            try
            {
                _userService.Register(RegisterUser);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("RegisterUser.Email", ex.Message);
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unable to register right now. Please try again.");
                return Page();
            }
            return RedirectToPage("/Auth/Login");
        }
    }
}
