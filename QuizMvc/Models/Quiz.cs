
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace QuizMvc.Models
{
     public class Quiz
    {
        [Key]
        public int QuizId { get; set; } 

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; } // Valgfri beskrivelse av quizen
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; } 

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!; // Navigasjonsegenskap
    
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public List<Question> Questions { get; set; } = new(); // Liste over spørsmål i quizen
    }

}