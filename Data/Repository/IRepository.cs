using Blog.Models;

using Blog.ViewModels;


namespace Blog.Data.Repository
{
    public interface IRepository
    {

        
        ArticleViewModel GetArticleViewModel(Guid id);
        IndexViewModel GetIndexViewModel(int pageNumber, string category, string search, string UserId);
        void AddArticle(Article article);
        bool UpdateArticle(Article article);
        void RemoveArticle(Guid id);
        void RemoveComment(Guid id);
        bool AddComment(Comment comment);
        bool AddView(Guid ArticleId, string UserId);
        bool AddArticleLike(Guid ArticleId, string UserId);
        bool AddCommentLike(Guid CommentId, string UserId);
        bool IsAllowedToPost(string UserId);
        ArticleViewModel GetFirstArticleByGenre(string Genre);
        Guid GetArticleId(Guid CommentId);
        Task<bool> SaveChangesAsync();
        int GetCommentlevelByID(Guid id);
        Article? GetArticle(Guid id);
        public Comment? GetComment(Guid commentId);
    }
}