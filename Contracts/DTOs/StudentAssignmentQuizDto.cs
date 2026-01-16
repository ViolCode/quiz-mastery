using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class StudentAssignmentQuizDto
    {
        public int AssignmentId { get; set; }
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
    }
}
