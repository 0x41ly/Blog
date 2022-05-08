using Blog.Models;

using Blog.ViewModels;


namespace Blog.Data.Repository
{
    public interface IRepository
    {

        
        ArticleViewModel GetArticleViewModel(Guid id);
        IndexViewModel GetIndexViewModel(int pageNumber, string category, string search, string UserId);
        void AddArticle(Article article);
        void UpdateArticle(Article article);
        void RemoveArticle(Guid id);
        void AddComment(Comment comment);
        void AddView(Guid ArticleId, string UserId);
        void AddArticleLike(Guid ArticleId, string UserId);
        void AddCommentLike(Guid CommentId, string UserId);
        bool IsAllowedToPost(string UserId);
        Task<bool> SaveChangesAsync();
    }
}