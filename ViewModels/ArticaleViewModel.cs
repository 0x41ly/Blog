using Blog.Areas.Identity.Data;
using Blog.Models;

namespace Blog.ViewModels
{
    public class ArticaleViewModel
    {

        public Article Article { get; set; }
        public BlogUser Author { get; set; }

        public List<CommentViewModel> CommentViewModels { get; set; }
    }
}