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
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly string _connectionString;

        public AssignmentRepository(string connectionString)
        {
            _connectionString = connectionString;
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

        public List<StudentAssignmentQuizDto> GetAssignmentsForStudent(int studentId)
        {
            var result = new List<StudentAssignmentQuizDto>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT ta.id AS assignment_id, q.id AS quiz_id, q.title, sa.assigned_at
                FROM student_assignment sa
                JOIN teacher_assignment ta ON ta.id = sa.assignment_id
                JOIN quiz q ON q.id = ta.quiz_id
                WHERE sa.student_id = @StudentId
                ORDER BY sa.assigned_at DESC", conn);

            cmd.Parameters.AddWithValue("@StudentId", studentId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new StudentAssignmentQuizDto
                {
                    AssignmentId = reader.GetInt32(0),
                    QuizId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    AssignedAt = reader.GetDateTime(3)
                });
            }

            return result;
        }
    }
}
