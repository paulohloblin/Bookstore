using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookstore;
using Bookstore.Models;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookstoreDbContext _context;

        public BooksController(BookstoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            if (!_context.Books.Any())
            {
                return NotFound();
            }

            return Ok(await _context.Books.ToListAsync());
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            if (!_context.Books.Any())
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                // Model validation failed; return a 400 Bad Request with error details.
                return BadRequest(ModelState);
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            if (!ModelState.IsValid)
            {
                // Model validation failed; return a 400 Bad Request with error details.
                return BadRequest(ModelState);
            }
            if (_context.Books == null)
          {
              return Problem("Entity set 'BookstoreDbContext.Books'  is null.");
          }
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Book>> SearchBooks([FromQuery] BookSearchModel searchModel)
        {
            var query = _context.Books
                .Include(book => book.Author)
                .Include(book => book.Genre)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
            {
                query = query.Where(book => book.Title.Contains(searchModel.Title));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Author))
            {
                query = query.Where(book => book.Author.Name.Contains(searchModel.Author));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Genre))
            {
                query = query.Where(book => book.Genre.Name.Contains(searchModel.Genre));
            }

            var result = query.ToList();
            return Ok(result);
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
