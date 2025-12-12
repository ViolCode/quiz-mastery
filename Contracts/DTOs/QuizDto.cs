using System;

namespace Contracts.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int VisibilityId { get; set; } = 1; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

