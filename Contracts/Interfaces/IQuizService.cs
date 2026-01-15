using Contracts.DTOs;

namespace Contracts.Interfaces
{
    public interface IQuizService
    {
        void CreateQuiz(int teacherId, CreateQuizRequestDTO request);
        List<QuizDto> GetQuizzesForStudent(int studentId);
        List<QuizDto> GetQuizzesForTeacher(int teacherId);
    }

    public class CreateQuizQuestionModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new() { "", "", "", "" };
        public int? CorrectOptionIndex { get; set; }
    }
}
