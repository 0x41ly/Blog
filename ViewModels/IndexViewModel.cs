using Blog.Models;
using System.Collections.Generic;

namespace Blog.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool NextPage { get; set; }
        public bool PreviousPage { get; set; }
        public string Genre { get; set; }
        public string Search { get; set; }
        public IEnumerable<ArticaleViewModel> RecommendedArticles { get; set; }
        public IEnumerable<ArticaleViewModel> MostViewedArticles { get; set; }
        public IEnumerable<ArticaleViewModel> Genres { get; set; }
        public IEnumerable<int> Pages { get; internal set; }
    }
}