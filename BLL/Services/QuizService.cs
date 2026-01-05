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

        public void CreateQuiz(string title, int visibilityId, List<CreateQuizQuestionModel> questions)
        {
            var quiz = new QuizDto
            {
                Title = title,
                VisibilityId = visibilityId,
                CreatedAt = DateTime.UtcNow
            };

            var quizId = _quizAccess.CreateQuiz(quiz);

            // Hardcode question_type_id to 1 (SingleChoice)
            const int singleChoiceTypeId = 1;

            foreach (var questionModel in questions)
            {
                var questionId = _quizAccess.CreateQuestion(quizId, questionModel.QuestionText, singleChoiceTypeId);

                int? correctAnswerId = null;

                for (int i = 0; i < questionModel.Options.Count; i++)
                {
                    var optionText = questionModel.Options[i];
                    var isCorrect = questionModel.CorrectOptionIndex == i;
                    
                    var answerId = _quizAccess.CreateAnswer(questionId, optionText, isCorrect);
                    
                    if (isCorrect)
                    {
                        correctAnswerId = answerId;
                    }
                }

                if (correctAnswerId.HasValue)
                {
                    _quizAccess.UpdateQuestionCorrectAnswer(questionId, correctAnswerId.Value);
                }
            }
        }
    }
}
