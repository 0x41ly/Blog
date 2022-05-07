using Blog.Models;

using Blog.ViewModels;


namespace Blog.Data.Repository
{
    public interface IRepository
    {

        
        ArticleViewModel GetArticleViewModel(Guid id);
        IndexViewModel GetAllArticles(int pageNumber, string category, string search);
        void AddArticle(Article article);
        void UpdateArticle(Article article);
        void RemoveArticle(Guid id);
        void AddComment(Comment comment);
        Task<bool> SaveChangesAsync();
    }
}