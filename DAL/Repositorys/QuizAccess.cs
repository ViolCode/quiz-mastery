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
                INSERT INTO [answer] (question_id, text, is_correct, student_assignment_id)
                OUTPUT INSERTED.id
                VALUES (@QuestionId, @Text, @IsCorrect, NULL)", conn);

            cmd.Parameters.AddWithValue("@QuestionId", questionId);
            cmd.Parameters.AddWithValue("@Text", answerText);
            cmd.Parameters.AddWithValue("@IsCorrect", isCorrect);

            return (int)cmd.ExecuteScalar();
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

        public int GetOrCreateAttempt(int studentId, int assignmentId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand(@"
                IF EXISTS (
                    SELECT 1 FROM dbo.student_quiz_attempt 
                    WHERE student_id = @StudentId AND assignment_id = @AssignmentId AND submitted_at IS NULL
                )
                BEGIN
                    SELECT id FROM dbo.student_quiz_attempt 
                    WHERE student_id = @StudentId AND assignment_id = @AssignmentId AND submitted_at IS NULL;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.student_quiz_attempt (student_id, assignment_id)
                    VALUES (@StudentId, @AssignmentId);
                    SELECT SCOPE_IDENTITY();
                END", conn);

            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@AssignmentId", assignmentId);

            return Convert.ToInt32(cmd.ExecuteScalar());
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

            // Get assignmentId for this attempt
            var assignmentCmd = new SqlCommand(@"
                SELECT assignment_id 
                FROM student_quiz_attempt 
                WHERE id = @AttemptId", conn);

            assignmentCmd.Parameters.AddWithValue("@AttemptId", attemptId);
            int assignmentId = (int)assignmentCmd.ExecuteScalar();

            // Check if quiz is already submitted
            var lockCmd = new SqlCommand(@"
                SELECT 1 
                FROM student_quiz_attempt 
                WHERE id = @AttemptId AND submitted_at IS NOT NULL", conn);

            lockCmd.Parameters.AddWithValue("@AttemptId", attemptId);
            if (lockCmd.ExecuteScalar() != null) return;

            // Clear previous answer
            var clearCmd = new SqlCommand(@"
                UPDATE answer
                SET student_assignment_id = NULL
                WHERE question_id = @QuestionId AND student_assignment_id = @AssignmentId", conn);

            clearCmd.Parameters.AddWithValue("@QuestionId", questionId);
            clearCmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
            clearCmd.ExecuteNonQuery();

            // Save new answer
            var selectCmd = new SqlCommand(@"
                UPDATE answer
                SET student_assignment_id = @AssignmentId
                WHERE id = @AnswerId", conn);

            selectCmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
            selectCmd.Parameters.AddWithValue("@AnswerId", answerId);
            selectCmd.ExecuteNonQuery();
        }

        public void SubmitAttempt(int attemptId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Get assignmentId
            var assignmentCmd = new SqlCommand(@"
                SELECT assignment_id 
                FROM student_quiz_attempt 
                WHERE id = @AttemptId", conn);

            assignmentCmd.Parameters.AddWithValue("@AttemptId", attemptId);
            int assignmentId = (int)assignmentCmd.ExecuteScalar();

            // Calculate score
            var scoreCmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM answer
                WHERE student_assignment_id = @AssignmentId AND is_correct = 1", conn);

            scoreCmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
            int score = (int)scoreCmd.ExecuteScalar();

            // Update attempt
            var update = new SqlCommand(@"
                UPDATE student_quiz_attempt
                SET submitted_at = SYSUTCDATETIME(), score = @Score
                WHERE id = @AttemptId", conn);

            update.Parameters.AddWithValue("@Score", score);
            update.Parameters.AddWithValue("@AttemptId", attemptId);
            update.ExecuteNonQuery();
        }
    }
}
