using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    [Keyless]
    public class RecommendedBy
    {

        [MaxLength(450)]
        public string UserId { get; set; }


        public Guid ArticleId { get; set; }
        /* [ForeignKey("UserId")]
         public BlogUser User { get; set; }*/
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }

    }
}