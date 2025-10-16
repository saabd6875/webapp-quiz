using System;
namespace QuizMvc.Models
{

    public class Answer
    {
        public int AnswerId { get; set; }
        public required string Text { get; set; } = ""; 
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}