using System.ComponentModel.DataAnnotations;

namespace QuizMvc.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Unik ID for kategorien
        
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; } = string.Empty; // Navnet på kategorien
    }
}