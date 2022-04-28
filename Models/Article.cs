using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class Article
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }
        [Required, MaxLength(50)]
        [ForeignKey("Standard")]
        public String Genre { get; set; }
        [Required, MaxLength(10)]
        public String Catagories { get; set; }
        [Required, MaxLength(15)]
        public String Level { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd,hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime Created { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd,hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdated { get; set; }
        [Required]
        public String Content { get; set; }
        public String CreatorId { get; set; }
        public List<Comment> comments = new();
        public int Views { get; set; }
        public List<Like> Likes = new();
        public int LikesCount { get; set; }
        public bool IsPremium { get; set; }

    }
}
