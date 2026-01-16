using Contracts.DTOs;

namespace Contracts.Interfaces
{
    public interface IQuizAccess
    {
        int CreateQuiz(QuizDto quiz);
        int CreateTeacherAssignment(int teacherId, int quizId);
        void AssignAssignmentToStudent(int studentId, int assignmentId);
        int CreateQuestion(int quizId, string questionText, int questionTypeId);
        int CreateAnswer(int questionId, string answerText, bool isCorrect);
        void UpdateQuestionCorrectAnswer(int questionId, int correctAnswerId);
        List<StudentAssignmentQuizDto> GetAssignmentsForStudent(int studentId);
        List<QuizDto> GetQuizzesCreatedByTeacher(int teacherId);
        StudentTakeQuizDto? GetQuizForStudent(int assignmentId, int studentId);
        int GetOrCreateAttempt(int studentId, int assignmentId);
        void SaveStudentAnswer(int attemptId, int questionId, int answerId);
        void SubmitAttempt(int attemptId);
        bool IsQuizAlreadySubmitted(int studentId, int assignmentId);
        bool AttemptIsSubmitted(int attemptId);
    }
}
