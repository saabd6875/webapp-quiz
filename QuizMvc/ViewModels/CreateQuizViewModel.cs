using QuizMvc.Models;

namespace QuizMvc.ViewModels
{
    // ViewModel for creating a new quiz
    public class CreateQuizViewModels
    {
     // This ViewModel is used to create or edit a quiz.
    // It holds all the data the user enters, including questions and images.
        public int QuizId { get; set; } 
        public string Title { get; set; } = string.Empty; // The title of the quiz
        public string? Description { get; set; } // Optional to add a description 
        //public int CategoryId { get; set; }
        public IFormFile? Image { get; set; } // Optional to add an image  for the quiz
        public List<QuestionInput> Questions { get; set; } = new();  // List of the questions the user adds to the quiz

        public class QuestionInput   
        {
            public string Text { get; set; } = string.Empty;  // The text of the question
             // the possible answer options
            public string OptionA { get; set; } = string.Empty;
            public string OptionB { get; set; } = string.Empty;
            public string OptionC { get; set; } = string.Empty;
            public string OptionD { get; set; } = string.Empty;
            public string CorrectOption { get; set; } = string.Empty; // The correct option for this question ("A", "B", "C", "D")
            public IFormFile? Image { get; set; } //optional to add an image to the question
            public string? ImageUrl { get; set; }

        } 
    }
}
