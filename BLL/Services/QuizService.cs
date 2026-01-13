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

            int quizId = _quizAccess.CreateQuiz(quiz);

            int assignmentId = _quizAccess.CreateTeacherAssignment(teacherId, quizId);

            const int singleChoiceTypeId = 1;

            foreach (var question in request.Questions)
            {
                int questionId = _quizAccess.CreateQuestion(
                    quizId,
                    question.QuestionText,
                    singleChoiceTypeId
                );

                int? correctAnswerId = null;

                for (int i = 0; i < question.Options.Count; i++)
                {
                    bool isCorrect = question.CorrectOptionIndex == i;
                    int answerId = _quizAccess.CreateAnswer(
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
                _quizAccess.AssignAssignmentToStudent(studentId, assignmentId);
            }
        }

        public List<QuizDto> GetQuizzesForStudent(int studentId)
        {
            return _quizAccess.GetQuizzesAssignedToStudent(studentId);
        }

        public List<QuizDto> GetQuizzesForTeacher(int teacherId)
        {
            return _quizAccess.GetQuizzesCreatedByTeacher(teacherId);
        }

    }
}
