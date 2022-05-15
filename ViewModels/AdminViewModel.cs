using Blog.Models;

namespace Blog.ViewModels
{
    public class AdminViewModel
    {
        public int UsersCount { get; set; }
        public int ArticleCount { get; set; }

        public UserProfile userProfile { get; set; }

        public MostInteractionsArticleViewModel MostInteractionsArticleViewModel { get; set; }
        public List<UserProfile>? UserRequestedPremium { get; set; }

    }
}
