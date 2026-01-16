using Contracts.DTOs;

namespace Contracts.Interfaces
{
    public interface IQuizService
    {
        void CreateQuiz(int teacherId, CreateQuizRequestDTO request);
        List<StudentAssignmentQuizDto> GetAssignmentsForStudent(int studentId);
        List<QuizDto> GetQuizzesForTeacher(int teacherId);
        StudentTakeQuizDto GetQuizForStudent(int assignmentId, int studentId);
        int StartQuiz(int studentId, int assignmentId);
        void SaveAnswer(int attemptId, int questionId, int answerId);
        void SubmitQuiz(int attemptId);
        bool IsQuizAlreadySubmitted(int studentId, int assignmentId);
    }
}
