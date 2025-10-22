using System;
namespace QuizMvc.Models
{

    public class Answer
    {
        public int AnswerId { get; set; }
        public required string Text { get; set; } = ""; 
        public bool IsCorrect { get; set; }
        public string OptionLetter { get; set; } = ""; // A, B, C, D

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}