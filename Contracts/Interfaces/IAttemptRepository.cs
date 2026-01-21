using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IAttemptRepository
    {
        int GetOrCreateAttempt(int studentId, int assignmentId);
        void SaveStudentAnswer(int attemptId, int questionId, int answerId);
        void SubmitAttempt(int attemptId, int score);
        bool AttemptIsSubmitted(int attemptId);
        bool IsQuizAlreadySubmitted(int studentId, int assignmentId);
        int CalculateScore(int attemptId);
    }
}
