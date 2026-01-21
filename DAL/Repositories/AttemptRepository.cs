using Contracts.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AttemptRepository : IAttemptRepository
    {
        private readonly string _connectionString;

        public AttemptRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int GetOrCreateAttempt(int studentId, int assignmentId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT id
                FROM student_quiz_attempt
                WHERE student_id = @StudentId
                    AND assignment_id = @AssignmentId", conn);

            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);

            var existing = cmd.ExecuteScalar();
            if (existing != null)
                return (int)existing;

            var insertCmd = new SqlCommand(@"
                INSERT INTO student_quiz_attempt (student_id, assignment_id)
                VALUES (@StudentId, @AssignmentId);
                SELECT SCOPE_IDENTITY();", conn);

            insertCmd.Parameters.AddWithValue("@StudentId", studentId);
            insertCmd.Parameters.AddWithValue("@AssignmentId", assignmentId);

            return Convert.ToInt32(insertCmd.ExecuteScalar());
        }

        public bool IsQuizAlreadySubmitted(int studentId, int assignmentId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT 1 
                FROM dbo.student_quiz_attempt 
                WHERE student_id = @StudentId AND assignment_id = @AssignmentId AND submitted_at IS NOT NULL", conn);

            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);

            return cmd.ExecuteScalar() != null;
        }

        public void SaveStudentAnswer(int attemptId, int questionId, int answerId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Remove previous answer for this question in this attempt
            var deleteCmd = new SqlCommand(@"
                DELETE FROM student_answer
                WHERE attempt_id = @AttemptId AND question_id = @QuestionId", conn);

            deleteCmd.Parameters.AddWithValue("@AttemptId", attemptId);
            deleteCmd.Parameters.AddWithValue("@QuestionId", questionId);
            deleteCmd.ExecuteNonQuery();

            // Insert new answer
            var insertCmd = new SqlCommand(@"
                INSERT INTO student_answer (attempt_id, question_id, answer_id)
                VALUES (@AttemptId, @QuestionId, @AnswerId)", conn);

            insertCmd.Parameters.AddWithValue("@AttemptId", attemptId);
            insertCmd.Parameters.AddWithValue("@QuestionId", questionId);
            insertCmd.Parameters.AddWithValue("@AnswerId", answerId);
            insertCmd.ExecuteNonQuery();
        }

        public void SubmitAttempt(int attemptId, int score)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                UPDATE student_quiz_attempt
                SET submitted_at = SYSUTCDATETIME(),
                    score = @Score
                WHERE id = @AttemptId", conn);

            cmd.Parameters.AddWithValue("@Score", score);
            cmd.Parameters.AddWithValue("@AttemptId", attemptId);

            cmd.ExecuteNonQuery();
        }

        public bool AttemptIsSubmitted(int attemptId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT 1
                FROM student_quiz_attempt
                WHERE id = @AttemptId
                  AND submitted_at IS NOT NULL", conn);

            cmd.Parameters.AddWithValue("@AttemptId", attemptId);

            return cmd.ExecuteScalar() != null;
        }

        public int CalculateScore(int attemptId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM student_answer sa
                JOIN answer a ON a.id = sa.answer_id
                WHERE sa.attempt_id = @AttemptId
                  AND a.is_correct = 1", conn);

            cmd.Parameters.AddWithValue("@AttemptId", attemptId);

            return (int)cmd.ExecuteScalar();
        }
    }
}
