namespace QuizMvc.ViewModels
{
    public class ViewQuizzesViewModel
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}