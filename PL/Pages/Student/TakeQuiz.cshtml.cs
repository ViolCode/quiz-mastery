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
    public class TakeQuizModel : PageModel
    {
        private readonly IQuizService _quizService;

        public StudentTakeQuizDto Quiz { get; set; } = null!;

        [BindProperty] public List<int> QuestionIds { get; set; } = new();
        [BindProperty] public List<int> Answers { get; set; } = new();
        [BindProperty] public int AttemptId { get; set; }
        [BindProperty] public int AssignmentId { get; set; }


        public TakeQuizModel(IQuizService quizService)
        {
            _quizService = quizService;
        }

        public IActionResult OnGet(int assignmentId)
        {
            int studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                Quiz = _quizService.GetQuizForStudent(assignmentId, studentId);
                if (Quiz == null)
                    return Forbid();

                AttemptId = _quizService.StartQuiz(studentId, assignmentId);
            }
            catch (InvalidOperationException ex)
            {
                TempData[$"Message_{assignmentId}"] = ex.Message;
                return RedirectToPage("/Student/MyAssignments");
            }


            return Page();
        }

        public IActionResult OnPost()
        {
            if (Answers.Count != QuestionIds.Count)
            {
                ModelState.AddModelError("", "All questions must be answered.");
                return Page();
            }

            try
            {
                for (int i = 0; i < QuestionIds.Count; i++)
                {
                    _quizService.SaveAnswer(AttemptId, QuestionIds[i], Answers[i]);
                }

                _quizService.SubmitQuiz(AttemptId);
            }
            catch (InvalidOperationException ex)
            {
                TempData[$"Message_{AssignmentId}"] = ex.Message;
                return RedirectToPage("/Student/MyAssignments");
            }

            return RedirectToPage("/Student/MyAssignments");
        }
    }
}
