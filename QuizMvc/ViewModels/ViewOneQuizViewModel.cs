namespace QuizMvc.ViewModels
{
    public class ViewOneQuizViewModel
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
