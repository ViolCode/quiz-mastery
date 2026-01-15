using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
using DAL.Models;
using DAL.Repositorys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PL.Pages.Teacher
{
    [Authorize(Roles = nameof(Role.Teacher))]
    public class CreateQuizModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly IUserService _userService;

        public CreateQuizModel(IQuizService quizService, IUserService userService)
        {
            _quizService = quizService;
            _userService = userService;
        }

        public List<UserDto> Students { get; set; } = new();

        [BindProperty]
        public List<int> SelectedStudentIds { get; set; } = new();

        [BindProperty]
        public string QuizTitle { get; set; } = string.Empty;

        [BindProperty]
        public List<CreateQuizQuestionModel> Questions { get; set; } = new()
        {
            new CreateQuizQuestionModel()
        };

        public void OnGet()
        {
            Students = _userService.GetAllStudents();
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

            Students = _userService.GetAllStudents();

            if (!ModelState.IsValid)
                return Page();

            var teacherIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(teacherIdClaim))
                return RedirectToPage("/Auth/Login");

            var teacherId = int.Parse(teacherIdClaim);

            var request = new CreateQuizRequestDTO
            {
                Title = QuizTitle,
                VisibilityId = 1, // Public for now
                Questions = Questions,
                AssignedStudentIds = SelectedStudentIds
            };

            _quizService.CreateQuiz(teacherId, request);

            TempData["Message"] = "Quiz successfully created.";
            return RedirectToPage("/Teacher/TeacherDashboard");
        }
    }
}
