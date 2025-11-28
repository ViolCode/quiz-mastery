using Contracts.DTOs;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PL.Pages
{
    public class IndexModel : PageModel
    {

        private readonly IUserAccess _userAccess;

        public IndexModel(IUserAccess userAccess)
        {
            _userAccess = userAccess;
        }

        public UserDto? CurrentUser { get; private set; }

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Auth/Login");
            }

            CurrentUser = _userAccess.GetUserByEmail(email); 
            return Page();
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Auth/Login");
        }
    }
}
