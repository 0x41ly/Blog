using Blog.Models;

using Blog.ViewModels;


namespace Blog.Data.Repository
{
    public interface IRepository
    {

        
        ArticleViewModel GetArticleViewModel(Guid id, string UserId);
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
        Guid? GetFirstArticleIdByGenre(string Genre);
        Guid GetArticleId(Guid CommentId);
        Task<bool> SaveChangesAsync();
        int GetCommentlevelByID(Guid id);
        Article? GetArticle(Guid id);

        bool Recommend(Guid ArticleId, string UserId);
        AdminViewModel AdminViewModel(string UserId);
        void RequestPremium(string UserId);
        void GivePremium(string UserId);

        bool RemoveUser(string UserId);
        string LocalPin(string UserId, Guid ArticleId);
        string GlobalPin(string UserId, Guid ArticleId);

    }
}