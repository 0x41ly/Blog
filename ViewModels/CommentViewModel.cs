using Blog.Areas.Identity.Data;
using Blog.Models;

namespace Blog.ViewModels
{
    public class CommentViewModel
    {
       
        public Comment Comment { get; set; }
        public BlogUser Author { get; set; }

        public List<CommentViewModel> CommentViewModels { get; set; }
    }
}