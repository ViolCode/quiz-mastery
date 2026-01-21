using Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Interfaces
{
    public interface IAssignmentRepository
    {
        int CreateTeacherAssignment(int teacherId, int quizId);
        void AssignAssignmentToStudent(int studentId, int assignmentId);
        List<StudentAssignmentQuizDto> GetAssignmentsForStudent(int studentId);
    }
}
