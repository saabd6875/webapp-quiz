using QuizMvc.Models;

namespace QuizMvc.ViewModels
{
    public class CreateQuizViewModels
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public List<QuestionInput> Questions { get; set; } = new();

        public class QuestionInput 
        {
            public string Text { get; set; } = string.Empty;
            public string OptionA { get; set; } = string.Empty;
            public string OptionB { get; set; } = string.Empty;
            public string OptionC { get; set; } = string.Empty;
            public string OptionD { get; set; } = string.Empty;
            public string CorrectOption { get; set; } = string.Empty;
            public IFormFile? Image { get; set; }
            public string? ImageUrl { get; set; }

        }
    }
}
