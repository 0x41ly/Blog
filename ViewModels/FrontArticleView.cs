using Blog.Models;

namespace Blog.ViewModels
{
    public class FrontArticleView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CommentsCount { get; set; }
        public UserProfile userProfile { get; set; }
        public int ViewsCount { get; set; }
    }
}