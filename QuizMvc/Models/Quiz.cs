
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

        public string? Description { get; set; } //optional description of the text
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!; //navigation property

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public List<Question> Questions { get; set; } = new(); //list of questions in quiz
    }
}