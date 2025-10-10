using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QuizMvc.Models
{
     public class Quiz
    {
        [Key]
        public int QuizId { get; set; } // Unik ID for quizen

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty; // Tittel på quizen
        
        public string? Description { get; set; } // Valgfri beskrivelse av quizen

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; } // Fremmednøkkel til Category

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!; // Navigasjonsegenskap
    
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public virtual List<Question> Questions { get; set; } = new List<Question>(); // Liste over spørsmål i quizen
    }

}