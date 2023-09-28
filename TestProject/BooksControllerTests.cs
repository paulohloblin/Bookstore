using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Bookstore.Controllers;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Bookstore;

namespace TestProject
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task GetBooks_ReturnsListOfBooks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.Add(new Book { Id = 1, Title = "Book 1" });
                context.Books.Add(new Book { Id = 2, Title = "Book 2" });
                context.Books.Add(new Book { Id = 3, Title = "Book 3" });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.GetBooks();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var books = Assert.IsType<List<Book>>(okResult.Value);
                Assert.Equal(3, books.Count);
            }
        }

        [Fact]
        public async Task GetBooks_ReturnsNotFound_WhenNoBooks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                // No books added to the database.
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.GetBooks();

                // Assert
                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        // Similar test methods for other actions: GetBook, PutBook, PostBook, DeleteBook.

        [Fact]
        public async Task PutBook_ReturnsBadRequest_WithInvalidModel()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.Add(new Book { Id = 1, Title = "Book 1", Price = 19.99m });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);
                controller.ModelState.AddModelError("Title", "The Title field is required.");
                controller.ModelState.AddModelError("Quantity", "The Quantity field is required.");

                // Create an invalid model with missing required properties.
                var invalidBook = new Book { Id = 1 }; // Missing Title and Quantity.

                // Act
                var result = await controller.PutBook(1, invalidBook);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                var modelState = badRequestResult.Value as SerializableError;
                Assert.NotNull(modelState);

                // Check that the model state contains validation errors for Title and Quantity.
                Assert.True(modelState.ContainsKey("Title"));
                Assert.True(modelState.ContainsKey("Quantity"));
            }
        }

        [Fact]
        public async Task PostBook_ReturnsBadRequest_WithInvalidModel()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);
                controller.ModelState.AddModelError("Title", "The Title field is required.");
                controller.ModelState.AddModelError("Quantity", "The Quantity field is required.");

                // Create an invalid model with missing required properties.
                var invalidBook = new Book { Price = 4 }; // Missing Title and Quantity.

                // Act
                var result = await controller.PostBook(invalidBook);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
                var modelState = badRequestResult.Value as SerializableError;
                Assert.NotNull(modelState);

                // Check that the model state contains validation errors for Title and Quantity.
                Assert.True(modelState.ContainsKey("Title"));
                Assert.True(modelState.ContainsKey("Quantity"));
            }
        }
        [Fact]
        public async Task PutBook_ReturnsBadRequest_WithNonMatchingIds()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.Add(new Book { Id = 1, Title = "Book 1", Price = 19.99m });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Create a model with a different ID.
                var nonMatchingBook = new Book { Id = 2, Title = "New Book", Price = 29.99m, Quantity = 10};

                // Act
                var result = await controller.PutBook(1, nonMatchingBook);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestResult>(result);
            }
        }
        [Fact]
        public async Task PutBook_ReturnsNotFound_WithNonExistentBook()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.Add(new Book { Id = 1, Title = "Book 1", Price = 19.99m });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Create a model for a book that doesn't exist.
                var nonExistentBook = new Book { Id = 2, Title = "Non-Existent Book", Price = 29.99m, Quantity = 10};

                // Act
                var result = await controller.PutBook(2, nonExistentBook);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result);
            }
        }
        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WithNonExistentBookId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.Add(new Book { Id = 1, Title = "Book 1", Price = 19.99m });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.DeleteBook(2); // Use a non-existent book ID.

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public void SearchBooks_ReturnsMatchingBooks()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.AddRange(new List<Book>
                {
                    new Book { Id = 1, Title = "Book 1", AuthorId = 1, GenreId = 1 },
                    new Book { Id = 2, Title = "Book 2", AuthorId = 2, GenreId = 2 },
                    new Book { Id = 3, Title = "Another Book", AuthorId = 1, GenreId = 2 }
                });
                context.Authors.AddRange(new List<Author>
                {
                    new Author { Id = 1, Name = "Author 1" },
                    new Author { Id = 2, Name = "Author 2" }
                });
                context.Genres.AddRange(new List<Genre>
                {
                    new Genre { Id = 1, Name = "Genre 1" },
                    new Genre { Id = 2, Name = "Genre 2" }
                });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var searchModel = new BookSearchModel { Title = "Book", Author = "Author 1" };
                var result = controller.SearchBooks(searchModel).Result;

                // Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                var books = Assert.IsAssignableFrom<IEnumerable<Book>>(okObjectResult.Value);
                Assert.Equal(2, books.Count());
                Assert.True(books.All(book => book.Title.Contains("Book") && book.Author.Name.Contains("Author 1")));
            }
        }

        [Fact]
        public async Task GetBook_ReturnsOkWithValidId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.Add(new Book { Id = 1, Title = "Book 1" });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.GetBook(1);

                // Assert
                var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
                var book = Assert.IsType<Book>(okObjectResult.Value);
                Assert.Equal("Book 1", book.Title);
            }
        }

        [Fact]
        public async Task GetBook_ReturnsNotFoundWithInvalidId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                context.Books.Add(new Book { Id = 1, Title = "Book 1" });
                context.SaveChanges();
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.GetBook(2); // Invalid ID.

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task GetBook_ReturnsNotFoundWhenDatabaseIsEmpty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<BookstoreDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new BookstoreDbContext(options))
            {
                // Database is empty.
            }

            using (var context = new BookstoreDbContext(options))
            {
                var controller = new BooksController(context);

                // Act
                var result = await controller.GetBook(1);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            }
        }
    }
}