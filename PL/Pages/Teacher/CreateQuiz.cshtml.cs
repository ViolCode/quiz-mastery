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
        public string QuestionText { get; set; } = string.Empty;

        [BindProperty]
        public List<string> Options { get; set; } = new() { "", "", "", "" };

        [BindProperty]
        public int? CorrectOptionIndex { get; set; }

        public string OptionError { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(QuizTitle))
            {
                ModelState.AddModelError(nameof(QuizTitle), "Quiz title is required.");
            }

            if (string.IsNullOrWhiteSpace(QuestionText))
            {
                ModelState.AddModelError(nameof(QuestionText), "Question text is required.");
            }

            for (int i = 0; i < Options.Count; i++)
            {
                Options[i] = Options[i]?.Trim() ?? string.Empty;
            }

            var filledOptions = Options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            if (filledOptions.Count < 2)
            {
                OptionError = "Please provide at least two answer options.";
                ModelState.AddModelError(nameof(Options), OptionError);
            }

            if (CorrectOptionIndex is null || CorrectOptionIndex < 0 || CorrectOptionIndex >= Options.Count || string.IsNullOrWhiteSpace(Options[CorrectOptionIndex.Value]))
            {
                OptionError = "Please select which option is correct.";
                ModelState.AddModelError(nameof(CorrectOptionIndex), OptionError);
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            TempData["Message"] = "Quiz was siccessfully created.";
            return RedirectToPage("/Teacher/TeacherDashboard");
        }
    }
}

