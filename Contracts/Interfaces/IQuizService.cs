namespace Contracts.Interfaces
{
    public interface IQuizService
    {
        void CreateQuiz(string title, int visibilityId, List<CreateQuizQuestionModel> questions);
    }

    public class CreateQuizQuestionModel
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new() { "", "", "", "" };
        public int? CorrectOptionIndex { get; set; }
    }
}
