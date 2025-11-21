using Contracts.DTOs;
using Contracts.Interfaces;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PL.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IUserAccess _userAccess;

        [BindProperty]
        public UserDto User { get; set; } = new UserDto();

        public RegisterModel(IUserService userService, IUserAccess userAccess)
        {
            _userService = userService;
            _userAccess = userAccess;
        }

        public List<SelectListItem> Roles { get; set; } = new();

        public void OnGet()
        {
            var roles = _userAccess.GetAllRoles();  
            Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) 
            { 
                return Page(); 
            }

            _userService.Register(User);
            return RedirectToPage("/Auth/Login");
        }
    }
}
