using System;

namespace QuizMvc.Models
{
    public class Question
    {
        public int QuestionId { get; set; } // Unik ID for spørsmålet
        public string Text { get; set; } = string.Empty; // Selve spørsmålet
        public List<string> Options { get; set; } = new List<string>(); // Liste over svaralternativer
        public string CorrectAnswer { get; set; } = string.Empty; // Riktig svar på spørsmålet
        public int QuizId { get; set; } // ID til quizen som dette spørsmålet hører til
    
    }

}