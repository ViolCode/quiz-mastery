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

                    int answerId = _quizAccess.CreateAnswer(questionId, optionText, isCorrect);

                    if (isCorrect)
                        correctAnswerId = answerId;
                }

                if (correctAnswerId.HasValue)
                {
                    _quizAccess.UpdateQuestionCorrectAnswer(questionId, correctAnswerId.Value);
                }
            }

            foreach (var studentId in request.AssignedStudentIds)
            {
                _quizAccess.AssignAssignmentToStudent(studentId, assignmentId);
            }
        }

        public List<StudentAssignmentQuizDto> GetAssignmentsForStudent(int studentId)
        {
            return _quizAccess.GetAssignmentsForStudent(studentId);
        }

        public List<QuizDto> GetQuizzesForTeacher(int teacherId)
        {
            return _quizAccess.GetQuizzesCreatedByTeacher(teacherId);
        }

        public StudentTakeQuizDto? GetQuizForStudent(int assignmentId, int studentId)
        {
            return _quizAccess.GetQuizForStudent(assignmentId, studentId);
        }

        public int StartQuiz(int studentId, int assignmentId)
        {
            return _quizAccess.GetOrCreateAttempt(studentId, assignmentId);
        }

        public void SaveAnswer(int attemptId, int questionId, int answerId)
        {
            _quizAccess.SaveStudentAnswer(attemptId, questionId, answerId);
        }

        public void SubmitQuiz(int attemptId)
        {
            _quizAccess.SubmitAttempt(attemptId);
        }
        public bool IsQuizAlreadySubmitted(int studentId, int assignmentId)
        {
            return _quizAccess.IsQuizAlreadySubmitted(studentId, assignmentId);
        }
    }
}
