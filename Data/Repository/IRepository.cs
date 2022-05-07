using Blog.Models;
<<<<<<< HEAD
using Blog.ViewModels;
=======

using Blog.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
>>>>>>> 48dfd1ae1383f3deb894105ba2ae175569d9bdf5

namespace Blog.Data.Repository
{
    public interface IRepository
    {
<<<<<<< HEAD
        
        ArticleViewModel GetArticleViewModel(Guid id);
=======
        Article GetArticle(Guid id);
        ArticaleViewModel GetArticleViewModel(Guid id);
        List<Article> GetAllArticles();
>>>>>>> 48dfd1ae1383f3deb894105ba2ae175569d9bdf5
        IndexViewModel GetAllArticles(int pageNumber, string category, string search);
        void AddArticle(Article article);
        void UpdateArticle(Article article);
        void RemoveArticle(Guid id);
        void AddComment(Comment comment);
        Task<bool> SaveChangesAsync();
    }
}