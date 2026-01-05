using Contracts.DTOs;
using Contracts.Interfaces;
using Microsoft.Data.SqlClient;

namespace DAL.Repositorys
{
    public class QuizAccess : IQuizAccess
    {
        private readonly string _connectionString;

        public QuizAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateQuiz(QuizDto quiz)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"
                INSERT INTO [quiz] (title, visibility_id, created_at)
                OUTPUT INSERTED.id
                VALUES (@Title, @VisibilityId, @CreatedAt)", conn);
            
            cmd.Parameters.AddWithValue("@Title", quiz.Title);
            cmd.Parameters.AddWithValue("@VisibilityId", quiz.VisibilityId);
            cmd.Parameters.AddWithValue("@CreatedAt", quiz.CreatedAt);
            
            var quizId = (int)cmd.ExecuteScalar();
            return quizId;
        }

        public int CreateQuestion(int quizId, string questionText, int questionTypeId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"
                INSERT INTO [question] (quiz_id, text, question_type_id, correct_answer_id)
                OUTPUT INSERTED.id
                VALUES (@QuizId, @Text, @QuestionTypeId, NULL)", conn);
            
            cmd.Parameters.AddWithValue("@QuizId", quizId);
            cmd.Parameters.AddWithValue("@Text", questionText);
            cmd.Parameters.AddWithValue("@QuestionTypeId", questionTypeId);
            
            var questionId = (int)cmd.ExecuteScalar();
            return questionId;
        }

        public int CreateAnswer(int questionId, string answerText, bool isCorrect)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"
                INSERT INTO [answer] (question_id, text, is_correct, student_assignment_id)
                OUTPUT INSERTED.id
                VALUES (@QuestionId, @Text, @IsCorrect, NULL)", conn);
            
            cmd.Parameters.AddWithValue("@QuestionId", questionId);
            cmd.Parameters.AddWithValue("@Text", answerText);
            cmd.Parameters.AddWithValue("@IsCorrect", isCorrect);
            
            var answerId = (int)cmd.ExecuteScalar();
            return answerId;
        }

        public void UpdateQuestionCorrectAnswer(int questionId, int correctAnswerId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"
                UPDATE [question]
                SET correct_answer_id = @CorrectAnswerId
                WHERE id = @QuestionId", conn);
            
            cmd.Parameters.AddWithValue("@QuestionId", questionId);
            cmd.Parameters.AddWithValue("@CorrectAnswerId", correctAnswerId);
            
            cmd.ExecuteNonQuery();
        }
    }
}
