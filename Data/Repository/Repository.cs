using Blog.Areas.Identity.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace Blog.Data.Repository
{
    public class Repository : IRepository
    {
        private static BlogDbContext _ctx;
        private readonly UserManager<BlogUser> _userManager;

        public Repository(BlogDbContext ctx,
            
            UserManager<BlogUser> userManager)
        {
            _ctx = ctx;

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
            
            

            var query = _ctx.Articles
                .OrderBy(a => a.Created)
                .AsNoTracking()
                .AsQueryable();

            if (!String.IsNullOrEmpty(category))
                query = query.Where(a => a.Categories.ToLower().Contains(category.ToLower() + ','));
                                                        

            if (!String.IsNullOrEmpty(search))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{search}%")
                                    || EF.Functions.Like(x.Content, $"%{search}%")
                                    || EF.Functions.Like(x.Description, $"%{search}%"));
            int pageSize = 5;
            int articlesCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)articlesCount / pageSize);
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            if (pageNumber > pageCount)
            {
                pageNumber = pageCount;
            }
            
            int skipAmount = pageSize * (pageNumber - 1);
            

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
                        Description = x.Description,
                        CreatedDate = x.Created,
                        CommentsCount = GetCommentsCount(x.ArticleId),
                        userProfile = GetUserProfile(x.AuthorId)
                    })
                    .Skip(skipAmount)
                    .Take(pageSize)
                    .ToList(),
                PinnedArticles = GetPinnedArticles(UserId),
                CategoriesCount = GetCategoriesCount()

            };
        }

        private List<CatagoryCountViewModel> GetCategoriesCount()
        {
            var categories = String.Join(',',_ctx.Articles
                                .Select(x => x.Categories)
                                .ToList())
                                .Split(',')
                                .Distinct();
            var CategoriesCount = new List<CatagoryCountViewModel>();
            foreach (var category in categories)
            {
                var categorycount = new CatagoryCountViewModel
                {
                    Category = category,
                    Count = GetCategoryCount(category)
                };
                CategoriesCount.Add(categorycount);
            }
            return CategoriesCount;
        }

        private int GetCategoryCount(string category)
        {
            return _ctx.Articles
                        .Where(a => a.Categories.ToLower().Contains(category.ToLower()+','))
                        .ToList()
                        .Count();
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
                                .Select(i => new 
                                { 
                                    ArticleId = i.Key,
                                    Count = i.Count()
                                })
                                .Where(a => a.Count >= _userManager.Users.Count()/2)
                                .ToList();
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
                    Description = x.Description,
                    CreatedDate = x.Created,
                    CommentsCount = GetCommentsCount(x.ArticleId),
                    userProfile = GetUserProfile(x.AuthorId)
                });
        }

        private static int GetCommentsCount(Guid articleId)
        {
            
            int count = _ctx.Comments.Where(a => a.ArticleId == articleId).Count();
            return count;
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

        public bool UpdateArticle(Article article)
        {
            if (GetArticle(article.ArticleId) != null)
            {
                _ctx.Articles.Update(article);
                return true;
            }
            return false;
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddComment(Comment comment)
        {
            if (GetComment(comment.CommentId) != null)
            {
                _ctx.Comments.Add(comment);
                return true;
            }
            return false;
        }

        public ArticleViewModel GetArticleViewModel(Guid id)
        {
            var ArticleViewModel = new ArticleViewModel();
            ArticleViewModel.Genres = GetGenres();
            ArticleViewModel.Article = GetArticle(id);
            if(ArticleViewModel.Article != null) 
            { 
                ArticleViewModel.ArticleLikes = GetArticleLikes(id);
                ArticleViewModel.ArticleViews = GetArticleViews(id);
                ArticleViewModel.Author = GetUserProfile(ArticleViewModel.Article.AuthorId);
                ArticleViewModel.SideBarArticles = GetSideBarArticles(ArticleViewModel.Article.GenreName);
                ArticleViewModel.MainComments = GetComments(id, 0);
            }
            else
            {
                ArticleViewModel.NotFound = true;
            }
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
                .Where(c => c.ArticleId == id & c.level == level).ToList();
            foreach (var comment in comments)
            {
                CommentViewModel commentViewModel = new CommentViewModel();
                commentViewModel.Comment = comment;
                commentViewModel.Creator = GetUserProfile(comment.AuthorId);
                commentViewModel.CommentLikes = GetCommentLikes(comment.CommentId);
                if (comment.level < 3)
                {
                    commentViewModel.SubComments = GetComments(id, comment.level +1 );
                }
                CommentsViewModdel.Add(commentViewModel);
            }

            return CommentsViewModdel;
        }
        public Article? GetArticle(Guid id)
        {
            return _ctx.Articles
                        .Where(a => a.ArticleId == id)
                        .FirstOrDefault();
        }
        private static UserProfile GetUserProfile(string id)
        {
            var user = _ctx.Users
                    .Where(u => u.Id == id)
                    .FirstOrDefault();
            UserProfile userProfile = new UserProfile
            {
                ProfilePicture = null ,
                PlanType = user.PlanType,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                DOB = user.DOB
            };
            return userProfile;
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

        public bool AddView(Guid ArticleId, string UserId)
        {
            if (GetArticle(ArticleId) != null)
            {
                var view = _ctx.Views.Where(e => e.ArticleId == ArticleId & e.UserId == UserId)
                    .FirstOrDefault();

                if (view == null)
                {
                    _ctx.Views.Add(new View {Id = Guid.NewGuid() , ArticleId = ArticleId, UserId = UserId });
                    return true;
                }
                
            }
            return false;
        }

        public bool AddArticleLike(Guid ArticleId, string UserId)
        {
            if (GetArticle(ArticleId) != null)
            {
                var ArticleLike = _ctx.ArticleLikes
                    .Where(e => e.ArticleId == ArticleId & e.UserId == UserId)
                    .FirstOrDefault();
                if (ArticleLike == null)
                {
                    ArticleLike = new ArticleLike
                    {
                        Id = Guid.NewGuid(),
                        ArticleId = ArticleId,
                        UserId = UserId
                    };
                    _ctx.ArticleLikes.Add(ArticleLike);
                    return true;
                }
                else
                {
                    _ctx.ArticleLikes.Remove(ArticleLike);
                    return true;
                }
                
            }
            return false;
        }

        public bool AddCommentLike(Guid CommentId, string UserId)
        {

            if (GetComment(CommentId) != null)
            {
                var CommentLike = _ctx.CommentLikes
                    .Where(e => e.CommentId == CommentId & e.UserId == UserId)
                    .FirstOrDefault();
                if (CommentLike == null)
                {
                     CommentLike = new CommentLike {
                        Id= Guid.NewGuid(),
                        CommentId = CommentId,
                        UserId = UserId };
                    _ctx.CommentLikes.Add(CommentLike);
                    return true;
                }
                else
                {
                    _ctx.CommentLikes.Remove(CommentLike);
                    return true;
                }
                
            }
            return false;
        }

        public Comment? GetComment(Guid commentId)
        {
            return _ctx.Comments
                .FirstOrDefault(e => e.CommentId == commentId);
        }

        public bool IsAllowedToPost(string UserId)
        {
            var MonthlyPostedArticles = _ctx.Articles
                                            .Where(a => a.AuthorId == UserId & a.Created.ToString("MM/yyyy") == DateTime.Now.ToString("MM/yyyy"))
                                            .ToList()
                                            .Count();
            return MonthlyPostedArticles < 2;
            
        }

        public ArticleViewModel GetFirstArticleByGenre(string Genre)
        {
            var article = _ctx.Articles
                .Where(a => a.GenreName == Genre)
                .OrderBy(a => a.Created)
                .ToList()[0];
            return GetArticleViewModel(article.ArticleId);
        }

        public int GetCommentlevelByID(Guid id)
        {
            var level = _ctx.Comments
                .Where(c => c.CommentId == id)
                .Select(c => c.level)
                .First();
            return level;
        }

        public Guid GetArticleId(Guid CommentId)
        {
            return GetComment(CommentId).ArticleId;
        }

        public void RemoveComment(Guid id)
        {
            _ctx.Comments.Remove(GetComment(id));
        }
    }
}