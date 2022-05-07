using Blog.Models;

namespace Blog.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool NextPage { get; set; }
        public bool PreviousPage { get; set; }
        
        public string Search { get; set; }
        public string Category { get; set; }
        public IEnumerable<Article> RecommendedArticles { get; set; }
        public IEnumerable<Article> MostViewedArticles { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public List<string> Genres { get; set; }
        public IEnumerable<int> Pages { get; internal set; }
    }
}