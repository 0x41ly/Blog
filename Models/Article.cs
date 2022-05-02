﻿using Blog.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class Article
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ArticleId { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }
        [Required, MaxLength(50)]
        
        public String GenreName { get; set; }
        [Required]
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
        [Required]
        public String Description { get; set; }
        
        [MaxLength(450)]
        public String AuthorId { get; set; }
        public bool Recommended { get; set; }

        public virtual BlogUser Author { get; set; }
        
        public virtual List<Comment> MainComments { get; set; }
        
  /*      public virtual ICollection<ArticleLike> ArticleLikes { get; set; }
        
        public virtual ICollection<View> Views { get; set; }*/

    }
}
