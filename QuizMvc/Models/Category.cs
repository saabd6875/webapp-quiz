using System.ComponentModel.DataAnnotations;

namespace QuizMvc.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Unique ID for the category
        
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; } = string.Empty; // category namee
    }
} 