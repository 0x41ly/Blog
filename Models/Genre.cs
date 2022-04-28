using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Genre
    {
        [Key]
        public String Name { get; set; }
    }
}
