using Blog.Areas.Identity.Data;
using Blog.Models;

namespace Blog.ViewModels
{
    public class ArticleViewModel
    {
        public List<string> Genres { get; set; }
        public Article Article { get; set; }
        public int ArticleLikes { get; set; }
        public int ArticleViews { get; set; }
        public UserProfile Author { get; set; }
        public List<FrontArticleView> SideBarArticles { get; set; }
        public List<CommentViewModel>? MainComments { get; set; }
    }
}