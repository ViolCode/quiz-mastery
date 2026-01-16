using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class StudentTakeQuizDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<StudentTakeQuestionDto> Questions { get; set; } = new();
    }
}
