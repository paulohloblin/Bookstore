using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name must not exceed 50 characters.")]
        public string Name { get; set; }

        // Navigation property for books in this genre
        public ICollection<Book> Books { get; set; }
    }

}
