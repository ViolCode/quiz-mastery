using Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IQuizRepository
    {
        int CreateQuiz(QuizDto quiz);
        int CreateQuestion(int quizId, string questionText, int questionTypeId);
        int CreateAnswer(int questionId, string answerText, bool isCorrect);
        void UpdateQuestionCorrectAnswer(int questionId, int correctAnswerId);
        List<QuizDto> GetQuizzesCreatedByTeacher(int teacherId);
        StudentTakeQuizDto? GetQuizForStudent(int assignmentId, int studentId);
    }
}
