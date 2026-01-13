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

        public int CreateTeacherAssignment(int teacherId, int quizId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO teacher_assignment (teacher_id, quiz_id, created_at)
                OUTPUT INSERTED.id
                VALUES (@TeacherId, @QuizId, GETUTCDATE())", conn);

            cmd.Parameters.AddWithValue("@TeacherId", teacherId);
            cmd.Parameters.AddWithValue("@QuizId", quizId);

            return (int)cmd.ExecuteScalar();
        }

        public void AssignAssignmentToStudent(int studentId, int assignmentId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO student_assignment (student_id, assignment_id, assigned_at)
                VALUES (@StudentId, @AssignmentId, GETUTCDATE())", conn);

            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);

            cmd.ExecuteNonQuery();
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

        public List<QuizDto> GetQuizzesAssignedToStudent(int studentId)
        {
            var quizzes = new List<QuizDto>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT DISTINCT
                    q.id,
                    q.title,
                    q.visibility_id,
                    q.created_at
                FROM quiz q
                JOIN teacher_assignment ta ON ta.quiz_id = q.id
                JOIN student_assignment sa ON sa.assignment_id = ta.id
                WHERE sa.student_id = @StudentId
                ORDER BY q.created_at DESC", conn);

            cmd.Parameters.AddWithValue("@StudentId", studentId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                quizzes.Add(new QuizDto
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    VisibilityId = reader.GetInt32(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return quizzes;
        }

        public List<QuizDto> GetQuizzesCreatedByTeacher(int teacherId)
        {
            var quizzes = new List<QuizDto>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT q.Id, q.Title, q.Visibility_Id, q.Created_At
                FROM quiz q
                JOIN teacher_assignment ta ON ta.quiz_id = q.id
                WHERE ta.teacher_id = @TeacherId
                ORDER BY q.Created_At DESC", conn);

            cmd.Parameters.AddWithValue("@TeacherId", teacherId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                quizzes.Add(new QuizDto
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    VisibilityId = reader.GetInt32(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return quizzes;
        }
    }
}
