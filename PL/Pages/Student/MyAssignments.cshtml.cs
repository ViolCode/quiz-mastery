using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PL.Pages.Student
{
    [Authorize(Roles = nameof(Role.Student))]
    public class MyAssignmentsModel : PageModel
    {
        private readonly IQuizService _quizService;

        public MyAssignmentsModel(IQuizService quizService)
        {
            _quizService = quizService;
        }

        public List<QuizDto> Quizzes { get; set; } = new();

        public IActionResult OnGet()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim))
                return RedirectToPage("/Auth/Login");

            int studentId = int.Parse(idClaim);
            Quizzes = _quizService.GetQuizzesForStudent(studentId);

            return Page();
        }
    }

}
