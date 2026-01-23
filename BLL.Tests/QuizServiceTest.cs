using BLL.Services;
using Contracts.DTOs;
using Contracts.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BLL.Tests
{
    public class QuizServiceTests
    {
        private readonly Mock<IQuizRepository> _quizRepoMock = new();
        private readonly Mock<IAssignmentRepository> _assignmentRepoMock = new();
        private readonly Mock<IAttemptRepository> _attemptRepoMock = new();

        private readonly QuizService _service;

        public QuizServiceTests()
        {
            _service = new QuizService(
                _quizRepoMock.Object,
                _assignmentRepoMock.Object,
                _attemptRepoMock.Object
            );
        }

        [Fact]
        public void CreateQuiz_ShouldCreateQuiz_Assignment_Questions_Answers()
        {
            var request = new CreateQuizRequestDTO
            {
                Title = "Test Quiz",
                VisibilityId = 1,
                Questions =
                {
                    new CreateQuizQuestionDTO
                    {
                        QuestionText = "Q1",
                        Options = new() { "A", "B", "C", "D" },
                        CorrectOptionIndex = 1
                    }
                },
                AssignedStudentIds = { 10, 11 }
            };

            _quizRepoMock.Setup(q => q.CreateQuiz(It.IsAny<QuizDto>()))
                .Returns(1);

            _assignmentRepoMock.Setup(a => a.CreateTeacherAssignment(5, 1))
                .Returns(100);

            _quizRepoMock.Setup(q => q.CreateQuestion(1, "Q1", 1))
                .Returns(50);

            _quizRepoMock.Setup(q => q.CreateAnswer(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(200);

            _service.CreateQuiz(5, request);

            _quizRepoMock.Verify(q => q.CreateQuiz(It.Is<QuizDto>(x => x.Title == "Test Quiz")), Times.Once);
            _assignmentRepoMock.Verify(a => a.CreateTeacherAssignment(5, 1), Times.Once);
            _quizRepoMock.Verify(q => q.CreateQuestion(1, "Q1", 1), Times.Once);
            _assignmentRepoMock.Verify(a => a.AssignAssignmentToStudent(10, 100), Times.Once);
            _assignmentRepoMock.Verify(a => a.AssignAssignmentToStudent(11, 100), Times.Once);
        }

        [Fact]
        public void GetAssignmentsForStudent_ShouldReturnAssignments()
        {
            var assignments = new List<StudentAssignmentQuizDto>
            {
                new() { AssignmentId = 1, QuizId = 2, Title = "Quiz" }
            };

            _assignmentRepoMock
                .Setup(a => a.GetAssignmentsForStudent(7))
                .Returns(assignments);

            var result = _service.GetAssignmentsForStudent(7);

            result.Should().BeEquivalentTo(assignments);
        }

        [Fact]
        public void GetQuizzesForTeacher_ShouldReturnQuizzes()
        {
            var quizzes = new List<QuizDto>
            {
                new() { Id = 1, Title = "Quiz 1" }
            };

            _quizRepoMock
                .Setup(q => q.GetQuizzesCreatedByTeacher(3))
                .Returns(quizzes);

            var result = _service.GetQuizzesForTeacher(3);

            result.Should().BeEquivalentTo(quizzes);
        }

        [Fact]
        public void StartQuiz_ShouldThrow_WhenAlreadySubmitted()
        {
            _attemptRepoMock
                .Setup(a => a.IsQuizAlreadySubmitted(2, 5))
                .Returns(true);

            Action act = () => _service.StartQuiz(2, 5);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Quiz already submitted.");
        }

        [Fact]
        public void StartQuiz_ShouldReturnAttemptId_WhenNotSubmitted()
        {
            _attemptRepoMock
                .Setup(a => a.IsQuizAlreadySubmitted(2, 5))
                .Returns(false);

            _attemptRepoMock
                .Setup(a => a.GetOrCreateAttempt(2, 5))
                .Returns(99);

            var result = _service.StartQuiz(2, 5);

            result.Should().Be(99);
        }

        [Fact]
        public void SaveAnswer_ShouldThrow_WhenAttemptSubmitted()
        {
            _attemptRepoMock
                .Setup(a => a.AttemptIsSubmitted(10))
                .Returns(true);

            Action act = () => _service.SaveAnswer(10, 1, 1);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Quiz already submitted.");
        }

        [Fact]
        public void SaveAnswer_ShouldCallRepository_WhenAttemptNotSubmitted()
        {
            _attemptRepoMock
                .Setup(a => a.AttemptIsSubmitted(10))
                .Returns(false);

            _service.SaveAnswer(10, 1, 2);

            _attemptRepoMock.Verify(a =>
                a.SaveStudentAnswer(10, 1, 2), Times.Once);
        }

        [Fact]
        public void SubmitQuiz_ShouldCalculateScore_AndSubmit()
        {
            _attemptRepoMock
                .Setup(a => a.AttemptIsSubmitted(15))
                .Returns(false);

            _attemptRepoMock
                .Setup(a => a.CalculateScore(15))
                .Returns(4);

            _service.SubmitQuiz(15);

            _attemptRepoMock.Verify(a => a.SubmitAttempt(15, 4), Times.Once);
        }

        [Fact]
        public void SubmitQuiz_ShouldThrow_WhenAlreadySubmitted()
        {
            _attemptRepoMock
                .Setup(a => a.AttemptIsSubmitted(15))
                .Returns(true);

            Action act = () => _service.SubmitQuiz(15);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Quiz already submitted.");
        }
    }
}
