using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Contracts.Enums;

namespace PL.Pages.Teacher
{
    [Authorize(Roles = nameof(Role.Teacher))]
    public class CreateQuizModel : PageModel
    {
        [BindProperty]
        public string QuizTitle { get; set; } = string.Empty;

        [BindProperty]
        public List<QuestionModel> Questions { get; set; } = new();

        public void OnGet()
        {
            // Initialize with one empty question
            if (Questions.Count == 0)
            {
                Questions.Add(new QuestionModel());
            }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(QuizTitle))
            {
                ModelState.AddModelError(nameof(QuizTitle), "Quiz title is required.");
            }

            if (Questions == null || Questions.Count == 0)
            {
                ModelState.AddModelError("", "At least one question is required.");
            }
            else
            {
                for (int q = 0; q < Questions.Count; q++)
                {
                    var question = Questions[q];
                    
                    if (string.IsNullOrWhiteSpace(question.QuestionText))
                    {
                        ModelState.AddModelError($"Questions[{q}].QuestionText", "Question text is required.");
                    }

                    if (question.Options == null)
                    {
                        question.Options = new List<string> { "", "", "", "" };
                    }

                    for (int i = 0; i < question.Options.Count; i++)
                    {
                        question.Options[i] = question.Options[i]?.Trim() ?? string.Empty;
                    }

                    var filledOptions = question.Options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
                    if (filledOptions.Count < 2)
                    {
                        ModelState.AddModelError($"Questions[{q}].Options", "Please provide at least two answer options.");
                    }

                    if (question.CorrectOptionIndex is null || 
                        question.CorrectOptionIndex < 0 || 
                        question.CorrectOptionIndex >= question.Options.Count || 
                        string.IsNullOrWhiteSpace(question.Options[question.CorrectOptionIndex.Value]))
                    {
                        ModelState.AddModelError($"Questions[{q}].CorrectOptionIndex", "Please select which option is correct.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                // Ensure at least one question exists for display
                if (Questions == null || Questions.Count == 0)
                {
                    Questions = new List<QuestionModel> { new QuestionModel() };
                }
                return Page();
            }

            TempData["Message"] = "Quiz was successfully created.";
            return RedirectToPage("/Teacher/TeacherDashboard");
        }
    }

    public class QuestionModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new() { "", "", "", "" };
        public int? CorrectOptionIndex { get; set; }
    }
}
