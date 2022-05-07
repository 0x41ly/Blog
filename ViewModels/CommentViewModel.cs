using Blog.Areas.Identity.Data;
using Blog.Models;

namespace Blog.ViewModels
{
    public class CommentViewModel
    {
       
        public Comment Comment { get; set; }
        public UserProfile Creator { get; set; }
        public int CommentLikes { get; set; }
        public List<CommentViewModel>? SubComments { get; set; }
    }
}