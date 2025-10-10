using System;
namespace QuizMvc.Models
{

    public class Answers
    {
        public int AnswerId { get; set; }
        public required string Tekst { get; set; } = string.Empty; 
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public required Question Question { get; set; }
    }
}