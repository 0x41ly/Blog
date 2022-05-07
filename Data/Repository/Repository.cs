using Blog.Areas.Identity.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private BlogDbContext _ctx;
        private readonly IMapper _mapper;

        public Repository(BlogDbContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }
        

        public void AddArticle(Article article)
        {
            _ctx.Articles.Add(article);
        }



        public IndexViewModel GetAllArticles(
            int pageNumber,
            string category,
            string search)
        {
            

            int pageSize = 5;
            int skipAmount = pageSize * (pageNumber - 1);

            var query = _ctx.Articles.AsNoTracking().AsQueryable();

            if (!String.IsNullOrEmpty(category))
                query = query.Where(a => ContainCatagory(a.Catagories, category));
                                                        

            if (!String.IsNullOrEmpty(search))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%")
                                    || EF.Functions.Like(x.Content, $"%{search}%")
                                    || EF.Functions.Like(x.Description, $"%{search}%"));

            int articlesCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)articlesCount / pageSize);

            return new IndexViewModel
            {
                Genres = GetGenres(),
                PageNumber = pageNumber,
                PageCount = pageCount,
                NextPage = pageNumber < pageCount,
                PreviousPage = pageNumber > 1,
                Pages = GetPageNumbers(pageNumber,pageCount),
                Category = category,
                Search = search,
                Articles = query
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList()
            };
        }

        private bool ContainCatagory(string catagories, string catagory)
        {
            return catagories.ToLower().Split(',').Contains(catagory.ToLower());
        }

        private List<int> GetPageNumbers(int pageNumber, int pageCount)
        {
            var pageNumbers = new List<int>();
            if (pageCount < 10)
            {
                for (int i = 0; i < pageCount; i++)
                {
                    pageNumbers.Add(i);
                }
            }
            else
            {
                if (pageNumber + 3 < pageCount & pageNumber - 3 > 1)
                {
                    pageNumbers.Add(1);
                    for (int i = pageNumber - 3; i < pageNumber + 4; i++)
                    {
                        pageNumbers.Add(i);
                    }
                    pageNumbers.Add(pageCount);
                }
                else if (pageNumber + 3 > pageCount & pageNumber - 3 > 1)
                {
                    pageNumbers.Add(1);
                    for (int i = pageCount - 7; i < pageCount + 1; i++)
                    {
                        pageNumbers.Add(i);
                    }

                }
                else
                {

                    for (int i = 1; i < 9; i++)
                    {
                        pageNumbers.Add(i);
                    }
                    pageNumbers.Add(pageCount);
                }
            }
            return pageNumbers;
        }

        public void RemoveArticle(Guid id)
        {
            _ctx.Articles.Remove(GetArticle(id));
        }

        public void UpdateArticle(Article article)
        {
            _ctx.Articles.Update(article);
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void AddComment(Comment comment)
        {
            _ctx.Comments.Add(comment);
            
        }

        public ArticleViewModel GetArticleViewModel(Guid id)
        {
            var ArticleViewModel = new ArticleViewModel();
            ArticleViewModel.Genres = GetGenres();
            ArticleViewModel.Article = GetArticle(id);
            ArticleViewModel.ArticleLikes = GetArticleLikes(id);
            ArticleViewModel.ArticleViews = GetArticleViews(id);
            ArticleViewModel.Author = GetUserProfile(ArticleViewModel.Article.AuthorId);
            ArticleViewModel.SideBarArticles = GetSideBarArticles(ArticleViewModel.Article.GenreName);
            ArticleViewModel.MainComments = GetComments(id, 0);                                        
                                                   
            return ArticleViewModel;
        }

        private int GetArticleViews(Guid id)
        {
            return _ctx.Views
                    .Where(a => a.ArticleId == id).ToList().Count();
        }

        private List<string> GetGenres()
        {
            return _ctx.Articles
                .OrderBy(a => a.GenreName)
                .Select(a => a.GenreName)
                .Distinct()
                .ToList();
        }

        private int GetArticleLikes(Guid id)
        {
            return _ctx.ArticleLikes
                    .Where(a => a.ArticleId == id).ToList().Count();
        }
        private int GetCommentLikes(Guid id)
        {
            return _ctx.CommentLikes
                    .Where(a => a.CommentId == id).ToList().Count();
        }
        private List<CommentViewModel>? GetComments(Guid id, int level)
        {
            var CommentsViewModdel = new List<CommentViewModel>();
            var comments = _ctx.Comments
                .Where(c => c.CommentId == id & c.level == level).ToList();
            foreach (var comment in comments)
            {
                CommentViewModel commentViewModel = new CommentViewModel();
                commentViewModel.Comment = comment;
                commentViewModel.Creator = GetUserProfile(comment.AuthorId);
                commentViewModel.CommentLikes = GetCommentLikes(comment.CommentId);
                if (comment.level < 3)
                {
                    commentViewModel.SubComments = GetComments(id, comment.level);
                }
                CommentsViewModdel.Add(commentViewModel);
            }

            return CommentsViewModdel;
        }
        private Article GetArticle(Guid id)
        {
            return _ctx.Articles
                        .Where(a => a.ArticleId == id)
                        .FirstOrDefault();
        }
        private UserProfile GetUserProfile(string id)
        {
            return _mapper.Map<UserProfile>(_ctx.Users
                    .Where(u => u.Id == id)
                    .FirstOrDefault());
        }
        private List<FrontArticleView> GetSideBarArticles(string genreName)
        {
            return _ctx.Articles
                    .Where(a => a.GenreName == genreName)
                    .Select(x => new FrontArticleView
                    {
                        Id = x.ArticleId,
                        Title = x.Title,
                        Description = x.Description
                    }).ToList();
        }
    }
}