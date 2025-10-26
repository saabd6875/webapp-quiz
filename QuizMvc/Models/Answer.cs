using System;
namespace QuizMvc.Models
{
    // This class represents a single  answer for a quiz question. Each answer belongs to one question
    public class Answer
    {
     // Primary key for the Answer table
        public int AnswerId { get; set; } 
        public required string Text { get; set; } = "";  //a required text for the answer 
        public bool IsCorrect { get; set; }   //  True if this is the correct answer for the question
        public string OptionLetter { get; set; } = ""; // letter identifier for the option (A, B, C, D)

        public int QuestionId { get; set; }  // The ID of the question this answer belongs to
        public Question Question { get; set; } = null!; // Navigation property to access the related question
    }
}