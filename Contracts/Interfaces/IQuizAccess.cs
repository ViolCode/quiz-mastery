using Contracts.DTOs;

namespace Contracts.Interfaces
{
    public interface IQuizAccess
    {
        int CreateQuiz(QuizDto quiz);
        int CreateQuestion(int quizId, string questionText, int questionTypeId);
        int CreateAnswer(int questionId, string answerText, bool isCorrect);
        void UpdateQuestionCorrectAnswer(int questionId, int correctAnswerId);
    }
}
