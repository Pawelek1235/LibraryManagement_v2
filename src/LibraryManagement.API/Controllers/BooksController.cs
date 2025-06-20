using LibraryManagement.Contracts;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public BooksController(IUnitOfWork uow)
            => _uow = uow;

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _uow.Books
                .Query()
                .Include(b => b.Author)
                .ToListAsync();

            var dtos = books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Isbn = b.ISBN,
                AuthorId = b.AuthorId,
                AuthorName = b.Author.FullName
            });

            return Ok(dtos);
        }

        // GET: api/books/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var b = await _uow.Books
                .Query()
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (b == null)
                return NotFound();

            var dto = new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Isbn = b.ISBN,
                AuthorId = b.AuthorId,
                AuthorName = b.Author.FullName
            };

            return Ok(dto);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = new Book
            {
                Title = input.Title,
                ISBN = input.Isbn,
                AuthorId = input.AuthorId
            };

            await _uow.Books.AddAsync(book);
            await _uow.SaveChangesAsync();

            // ponownie pobieramy z autorem, by mieć FullName
            var created = await _uow.Books
                .Query()
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == book.Id);

            var result = new BookDto
            {
                Id = created!.Id,
                Title = created.Title,
                Isbn = created.ISBN,
                AuthorId = created.AuthorId,
                AuthorName = created.Author.FullName
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }

        // PUT: api/books/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookDto input)
        {
            if (!ModelState.IsValid || id != input.Id)
                return BadRequest(ModelState);

            var existing = await _uow.Books.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.Title = input.Title;
            existing.ISBN = input.Isbn;
            existing.AuthorId = input.AuthorId;

            _uow.Books.Update(existing);
            await _uow.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/books/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _uow.Books.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            _uow.Books.Delete(existing);
            await _uow.SaveChangesAsync();

            return NoContent();
        }
    }
}
