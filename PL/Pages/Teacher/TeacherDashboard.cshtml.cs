using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PL.Pages.Teacher
{
    [Authorize(Roles = nameof(Role.Teacher))]
    public class TeacherDashboardModel : PageModel
    {
        private readonly IUserAccess _userAccess;

        public TeacherDashboardModel(IUserAccess userAccess)
        {
            _userAccess = userAccess;
        }

        public UserDto? CurrentUser { get; private set; }

        public IActionResult OnGet()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim))
                return RedirectToPage("/Auth/Login");

            var userId = int.Parse(idClaim);

            CurrentUser = _userAccess.GetUserById(userId);
            return Page();
        }
    }
}
