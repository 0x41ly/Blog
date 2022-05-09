using Blog.Models;

namespace Blog.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool NextPage { get; set; }
        public bool PreviousPage { get; set; }
        
        public string? Search { get; set; }
        public string? Category { get; set; }
        public IEnumerable<FrontArticleView>? RecommendedArticles { get; set; }
        public IEnumerable<FrontArticleView>? PinnedArticles { get; set; }
        public IEnumerable<FrontArticleView>? Articles { get; set; }
        public List<string>? Genres { get; set; }
        public List<CatagoryCountViewModel>? CategoriesCount { get; set; }
        public IEnumerable<int>? Pages { get; internal set; }
    }
}