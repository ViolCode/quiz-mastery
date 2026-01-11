using Contracts.DTOs;
using Contracts.Interfaces;

namespace BLL.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizAccess _quizAccess;

        public QuizService(IQuizAccess quizAccess)
        {
            _quizAccess = quizAccess;
        }

        public void CreateQuiz(int teacherId, CreateQuizRequestDTO request)
        {
            var quiz = new QuizDto
            {
                Title = request.Title,
                VisibilityId = request.VisibilityId,
                CreatedAt = DateTime.UtcNow
            };

            var quizId = _quizAccess.CreateQuiz(quiz);

            _quizAccess.CreateTeacherAssignment(teacherId, quizId);

            const int singleChoiceTypeId = 1;

            foreach (var question in request.Questions)
            {
                var questionId = _quizAccess.CreateQuestion(
                    quizId,
                    question.QuestionText,
                    singleChoiceTypeId
                );

                int? correctAnswerId = null;

                for (int i = 0; i < question.Options.Count; i++)
                {
                    var isCorrect = question.CorrectOptionIndex == i;

                    var answerId = _quizAccess.CreateAnswer(
                        questionId,
                        question.Options[i],
                        isCorrect
                    );

                    if (isCorrect)
                        correctAnswerId = answerId;
                }

                if (correctAnswerId.HasValue)
                {
                    _quizAccess.UpdateQuestionCorrectAnswer(
                        questionId,
                        correctAnswerId.Value
                    );
                }
            }

            foreach (var studentId in request.AssignedStudentIds.Distinct())
            {
                _quizAccess.AssignQuizToStudent(studentId, quizId);
            }
        }
    }
}
