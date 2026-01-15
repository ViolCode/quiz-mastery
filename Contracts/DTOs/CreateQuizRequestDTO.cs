using Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class CreateQuizRequestDTO
    {
        public string Title { get; set; } = string.Empty;
        public int VisibilityId { get; set; }
        public List<CreateQuizQuestionModel> Questions { get; set; } = new();
        public List<int> AssignedStudentIds { get; set; } = new();
    }
}
