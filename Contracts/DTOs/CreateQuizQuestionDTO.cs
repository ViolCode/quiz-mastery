using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class CreateQuizQuestionDTO
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new() { "", "", "", "" };
        public int? CorrectOptionIndex { get; set; }
    }
}
