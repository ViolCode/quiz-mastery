using Contracts.DTOs;
using Contracts.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly string _connectionString;

        public QuizRepository(string connectionString)
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

            return (int)cmd.ExecuteScalar();
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

            return (int)cmd.ExecuteScalar();
        }

        public int CreateAnswer(int questionId, string answerText, bool isCorrect)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO [answer] (question_id, text, is_correct)
                OUTPUT INSERTED.id
                VALUES (@QuestionId, @Text, @IsCorrect)", conn);

            cmd.Parameters.AddWithValue("@QuestionId", questionId);
            cmd.Parameters.AddWithValue("@Text", answerText);
            cmd.Parameters.AddWithValue("@IsCorrect", isCorrect);

            return (int)cmd.ExecuteScalar();
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

        public StudentTakeQuizDto? GetQuizForStudent(int assignmentId, int studentId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Ensure quiz belongs to student
            var quizCmd = new SqlCommand(@"
                SELECT q.id, q.title
                FROM student_assignment sa
                JOIN teacher_assignment ta ON ta.id = sa.assignment_id
                JOIN quiz q ON q.id = ta.quiz_id
                WHERE sa.student_id = @StudentId AND sa.assignment_id = @AssignmentId", conn);

            quizCmd.Parameters.AddWithValue("@StudentId", studentId);
            quizCmd.Parameters.AddWithValue("@AssignmentId", assignmentId);

            using var reader = quizCmd.ExecuteReader();
            if (!reader.Read())
                return null;

            var quiz = new StudentTakeQuizDto
            {
                AssignmentId = assignmentId,
                Title = reader.GetString(1)
            };
            reader.Close();

            // Load questions and answers
            var cmd = new SqlCommand(@"
                SELECT q.id, q.text, a.id, a.text
                FROM question q
                JOIN answer a ON a.question_id = q.id
                WHERE q.quiz_id = (SELECT quiz_id FROM teacher_assignment WHERE id = @AssignmentId)", conn);

            cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);

            using var qReader = cmd.ExecuteReader();
            var map = new Dictionary<int, StudentTakeQuestionDto>();

            while (qReader.Read())
            {
                int qId = qReader.GetInt32(0);
                if (!map.ContainsKey(qId))
                {
                    map[qId] = new StudentTakeQuestionDto
                    {
                        QuestionId = qId,
                        Text = qReader.GetString(1)
                    };
                }

                map[qId].Answers.Add(new StudentTakeAnswerDto
                {
                    AnswerId = qReader.GetInt32(2),
                    Text = qReader.GetString(3)
                });
            }

            quiz.Questions = map.Values.ToList();
            return quiz;
        }
    }
}
