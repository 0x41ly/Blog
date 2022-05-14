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
        Comment? GetComment(Guid commentId);
        bool Recommend(Guid ArticleId, string UserId);
        AdminViewModel AdminViewModel(string UserId);
        void RequestPremium(string UserId);
        void GivePremium(string UserId);

        bool RemoveUser(string UserId);
        bool LocalPin(string UserId, Guid ArticleId);
        bool GlobalPin(string UserId, Guid ArticleId);

    }
}