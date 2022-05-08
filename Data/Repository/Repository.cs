using Blog.Areas.Identity.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Identity;


namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private BlogDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly UserManager<BlogUser> _userManager;

        public Repository(BlogDbContext ctx,
            IMapper mapper,
            UserManager<BlogUser> userManager)
        {
            _ctx = ctx;
            _mapper = mapper;
            _userManager = userManager;
        }
        

        public void AddArticle(Article article)
        {
            _ctx.Articles.Add(article);
        }



        public IndexViewModel GetIndexViewModel(
            int pageNumber,
            string category,
            string search,
            string UserId)
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
                Pages = GetPageNumbers(pageNumber, pageCount),
                Category = category,
                Search = search,
                RecommendedArticles = GetRecommenedArticles(),
                Articles = (List<FrontArticleView>) query
                    .Select(x => new FrontArticleView
                    {
                        Id = x.ArticleId,
                        Title = x.Title,
                        Description = x.Description
                    })
                    .Skip(articlesCount)
                    .Take(pageSize)
                    .ToList(),
                PinnedArticles = GetPinnedArticles(UserId)

            };
        }

        private IEnumerable<FrontArticleView> GetPinnedArticles(string UserId)
        {
            var AdminPinnedArticles = _ctx.Articles
                .Where(x => x.Pinned)
                .Select(x => new FrontArticleView
                {
                    Id = x.ArticleId,
                    Title = x.Title,
                    Description = x.Description
                }).ToList();

            var UserPinnedArticles = new List<FrontArticleView>();
            if (UserId != null)
            {
                var UserPinnedArticlesId = _ctx.PinnedArticles
                                            .Where(x => x.UserId == UserId)
                                            .Select(i => new PinnedArticles {
                                                ArticleId = i.ArticleId   
                                            });
                foreach (var UserPinnedArticle in UserPinnedArticlesId)
                {
                    UserPinnedArticles.Add(GetFrontArticleViewById(UserPinnedArticle.ArticleId));
                }
            }

            var PinnedArticles = AdminPinnedArticles.Union(UserPinnedArticles).ToList();
            return PinnedArticles;
        }

        private IEnumerable<FrontArticleView> GetRecommenedArticles()
        {
           var RecommendedArticlesId = _ctx.RecommendedBy
                                .GroupBy(e => e.ArticleId)
                                .Select(i => new RecommendedBy
                                { 
                                    ArticleId = i.Key,
                                    Count = i.Count()
                                })
                                .Where(a => a.Count >= _userManager.Users.Count()/2).ToList();
            var RecommendedArticles = new List<FrontArticleView>();
            foreach (var article in RecommendedArticlesId)
            {
                RecommendedArticles.Add(GetFrontArticleViewById(article.ArticleId));
            }
            return RecommendedArticles;
        }

        private FrontArticleView GetFrontArticleViewById(Guid articleId)
        {
            return (FrontArticleView)_ctx.Articles
                .Where(a => a.ArticleId == articleId)
                .Select(x => new FrontArticleView
                {
                    Id = x.ArticleId,
                    Title = x.Title,
                    Description = x.Description
                });
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

        public void AddView(Guid ArticleId, string UserId)
        {
            
            if (!_ctx.Views.Any(e => e.ArticleId == ArticleId & e.UserId == UserId))
            {
                _ctx.Views.Add(new View { ArticleId = ArticleId, UserId = UserId });
            }
            
        }

        public void AddArticleLike(Guid ArticleId, string UserId)
        {
            var ArticleLike = new ArticleLike { ArticleId = ArticleId, UserId = UserId };
            if (!_ctx.ArticleLikes.Any(e => e.ArticleId == ArticleId & e.UserId == UserId))
            {
                _ctx.ArticleLikes.Add(ArticleLike);
            }
            else
            {
                _ctx.ArticleLikes.Remove(ArticleLike);
            }
        }

        public void AddCommentLike(Guid CommentId, string UserId)
        {
            var CommentLike = new CommentLike { CommentId = CommentId, UserId = UserId }; 
            if (_ctx.CommentLikes.Any(e => e.CommentId == CommentId & e.UserId == UserId))
            {
                _ctx.CommentLikes.Add(CommentLike);
            }
            else
            {
                _ctx.CommentLikes.Remove(CommentLike);
            }
        }

        public bool IsAllowedToPost(string UserId)
        {
            var MonthlyPostedArticles = _ctx.Articles
                                            .Where(a => a.AuthorId == UserId & a.Created.ToString("MM/yyyy") == DateTime.Now.ToString("MM/yyyy"))
                                            .ToList()
                                            .Count();
            return MonthlyPostedArticles < 2;
            
        }
    }
}