using Contracts.DTOs;
using Contracts.Interfaces;

namespace BLL.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAttemptRepository _attemptRepository;

        public QuizService(IQuizRepository quizRepository, IAssignmentRepository assignmentRepository, IAttemptRepository attemptRepository)
        {
            _quizRepository = quizRepository;
            _assignmentRepository = assignmentRepository;
            _attemptRepository = attemptRepository;
        }

        public void CreateQuiz(int teacherId, CreateQuizRequestDTO request)
        {
            var quiz = new QuizDto{
                Title = request.Title,
                VisibilityId = request.VisibilityId,
                CreatedAt = DateTime.UtcNow
            };

            int quizId = _quizRepository.CreateQuiz(quiz);
            int assignmentId = _assignmentRepository.CreateTeacherAssignment(teacherId, quizId);

            foreach (var question in request.Questions)
            {
                int questionId = _quizRepository.CreateQuestion(quizId, question.QuestionText, 1);

                int? correctAnswerId = null;
                for (int i = 0; i < question.Options.Count; i++)
                {
                    var optionText = question.Options[i];
                    if (string.IsNullOrWhiteSpace(optionText)) continue;

                    bool isCorrect = question.CorrectOptionIndex == i;
                    int answerId = _quizRepository.CreateAnswer(questionId, optionText, isCorrect);

                    if (isCorrect)
                        correctAnswerId = answerId;
                }

                if (correctAnswerId.HasValue)
                    _quizRepository.UpdateQuestionCorrectAnswer(questionId, correctAnswerId.Value);
            }

            foreach (var studentId in request.AssignedStudentIds)
            {
                _assignmentRepository.AssignAssignmentToStudent(studentId, assignmentId);
            }
        }

        public List<StudentAssignmentQuizDto> GetAssignmentsForStudent(int studentId)
            => _assignmentRepository.GetAssignmentsForStudent(studentId);

        public List<QuizDto> GetQuizzesForTeacher(int teacherId)
            => _quizRepository.GetQuizzesCreatedByTeacher(teacherId);

        public StudentTakeQuizDto? GetQuizForStudent(int assignmentId, int studentId)
            => _quizRepository.GetQuizForStudent(assignmentId, studentId);

        public int StartQuiz(int studentId, int assignmentId)
        {
            if (_attemptRepository.IsQuizAlreadySubmitted(studentId, assignmentId))
                throw new InvalidOperationException("Quiz already submitted.");

            return _attemptRepository.GetOrCreateAttempt(studentId, assignmentId);
        }

        public void SaveAnswer(int attemptId, int questionId, int answerId)
        {
            if (_attemptRepository.AttemptIsSubmitted(attemptId))
                throw new InvalidOperationException("Quiz already submitted.");

            _attemptRepository.SaveStudentAnswer(attemptId, questionId, answerId);
        }

        public void SubmitQuiz(int attemptId)
        {
            if (_attemptRepository.AttemptIsSubmitted(attemptId))
                throw new InvalidOperationException("Quiz already submitted.");

            int score = _attemptRepository.CalculateScore(attemptId);
            _attemptRepository.SubmitAttempt(attemptId, score);
        }

        public bool IsQuizAlreadySubmitted(int studentId, int assignmentId)
            => _attemptRepository.IsQuizAlreadySubmitted(studentId, assignmentId);
    }
}
