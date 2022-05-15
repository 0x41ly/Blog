namespace Blog.ViewModels
{
    public class MostInteractionsArticleViewModel
    {
        public IEnumerable<FrontArticleView>? MostLikedArticles { get; set; }
        public IEnumerable<FrontArticleView>? MostCommentedArticles { get; set; }

        public IEnumerable<FrontArticleView>? MostViewedArticles { get; set; }
    }
}
