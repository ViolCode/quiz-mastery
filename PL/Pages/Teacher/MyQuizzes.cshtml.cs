using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PL.Pages.Teacher
{
    [Authorize(Roles = nameof(Role.Teacher))]
    public class MyQuizzesModel : PageModel
    {
        private readonly IQuizService _quizService;

        public MyQuizzesModel(IQuizService quizService)
        {
            _quizService = quizService;
        }

        public List<QuizDto> Quizzes { get; set; } = new();

        public IActionResult OnGet()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim))
                return RedirectToPage("/Auth/Login");

            int teacherId = int.Parse(idClaim);
            Quizzes = _quizService.GetQuizzesForTeacher(teacherId);

            return Page();
        }
    }

}
