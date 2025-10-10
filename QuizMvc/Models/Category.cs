using System.ComponentModel.DataAnnotations;

namespace QuizMvc.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Unik ID for kategorien
        public string Name { get; set; } = string.Empty; // Navnet på kategorien
    }
}