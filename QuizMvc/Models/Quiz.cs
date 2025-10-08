using System;

//here we have the model for the quiz- change it so it works for SQLite
namespace QuizMvc.Models
{
     public class Quiz
    {
        public int QuizId { get; set; } // Unik ID for quizen
        public string Title { get; set; } = string.Empty; // Tittel på quizen
        public string? Description { get; set; } // Valgfri beskrivelse av quizen
        public List<Question> Questions { get; set; } = new List<Question>(); // Liste over spørsmål i quizen
    }

}