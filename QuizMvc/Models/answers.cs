using System;

namespace QuizMvc.Models
{

    public class Answers
    {
        public int AnswerId { get; set; }
        public string Tekst { get; set; }
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}