using Microsoft.EntityFrameworkCore.Design;

namespace QuizMvc.ViewModels
{
  public class TakeQuizViewModels
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<QuestionItem> Question { get; set; } = new();
        public List<string> UserAnswers { get; set; } = new();

        public class QuestionItem
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
            public string OptionA { get; set; } = string.Empty;
            public string OptionB { get; set; } = string.Empty;
            public string OptionC { get; set; } = string.Empty;
            public string OptionD { get; set; } = string.Empty; 
            
        }
    
    }
}
