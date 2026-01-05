using Contracts.Enums;
using Contracts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PL.Pages.Teacher
{
    [Authorize(Roles = nameof(Role.Teacher))]
    public class CreateQuizModel : PageModel
    {
        private readonly IQuizService _quizService;

        public CreateQuizModel(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [BindProperty]
        public string QuizTitle { get; set; } = string.Empty;

        [BindProperty]
        public List<CreateQuizQuestionModel> Questions { get; set; } = new()
        {
            new CreateQuizQuestionModel()
        };

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(QuizTitle))
            {
                ModelState.AddModelError(nameof(QuizTitle), "Quiz title is required.");
            }

            if (Questions == null || Questions.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "At least one question is required.");
            }

            foreach (var q in Questions)
            {
                if (string.IsNullOrWhiteSpace(q.QuestionText))
                {
                    ModelState.AddModelError(string.Empty, "All questions must have text.");
                }

                var filledOptions = q.Options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
                if (filledOptions.Count < 2)
                {
                    ModelState.AddModelError(string.Empty, "Each question must have at least two options.");
                }

                if (q.CorrectOptionIndex == null ||
                    q.CorrectOptionIndex < 0 ||
                    q.CorrectOptionIndex >= q.Options.Count)
                {
                    ModelState.AddModelError(string.Empty, "Each question must have a correct answer selected.");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // VisibilityId: 1 = Public (you can wire UI later)
            _quizService.CreateQuiz(
                QuizTitle,
                visibilityId: 1,
                questions: Questions
            );

            TempData["Message"] = "Quiz successfully created.";
            return RedirectToPage("/Teacher/TeacherDashboard");
        }
    }
}
