using Contracts.DTOs;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PL.Pages.Student
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardModel : PageModel
    {
        private readonly IUserAccess _userAccess;

        public StudentDashboardModel(IUserAccess userAccess)
        {
            _userAccess = userAccess;
        }

        public UserDto? CurrentUser { get; private set; }

        public IActionResult OnGet()
        {
            string email = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Auth/Login");

            CurrentUser = _userAccess.GetUserByEmail(email);
            return Page();
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await HttpContext.SignOutAsync("MyCookie"); 
            return RedirectToPage("/Auth/Login");
        }

    }
}
