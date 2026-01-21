using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PL.Pages.Student
{
    [Authorize(Roles = nameof(Role.Student))]
    public class StudentDashboardModel : PageModel
    {
        private readonly IUserRepository _userAccess;

        public StudentDashboardModel(IUserRepository userAccess)
        {
            _userAccess = userAccess;
        }

        public UserDto? CurrentUser { get; private set; }

        public IActionResult OnGet()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim))
            {
                return RedirectToPage("/Auth/Login");
            }

            int userId = int.Parse(idClaim);
            CurrentUser = _userAccess.GetUserById(userId);

            if (CurrentUser == null)
            {
                // user deleted or corrupted cookie
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }
    }
}
