using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        // Navigation property for books written by this author
        public ICollection<Book> Books { get; set; }
    }

}
