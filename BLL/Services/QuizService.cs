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

            // 3. Create questions and answers
            foreach (var question in request.Questions)
            {
                int questionId = _quizAccess.CreateQuestion(
                    quizId,
                    question.QuestionText,
                    questionTypeId: 1 // SingleChoice
                );

                int? correctAnswerId = null;

                for (int i = 0; i < question.Options.Count; i++)
                {
                    var optionText = question.Options[i];
                    if (string.IsNullOrWhiteSpace(optionText))
                        continue;

                    bool isCorrect = question.CorrectOptionIndex == i;

                    int answerId = _quizAccess.CreateAnswer(
                        questionId,
                        optionText,
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

            // 4. Assign quiz to selected students
            foreach (var studentId in request.AssignedStudentIds)
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
