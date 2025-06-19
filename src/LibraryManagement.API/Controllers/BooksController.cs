// LibraryManagement.API/Controllers/BooksController.cs
using LibraryManagement.API.Models;
using LibraryManagement.Core.DTOs;
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
        public async Task<IEnumerable<BookDto>> GetAll()
        {
            return await _uow.Books
                .Query()
                .Include(b => b.Author)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    AuthorName = b.Author.FullName
                })
                .ToListAsync();
        }

        // GET: api/books/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _uow.Books
                .Query()
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.Author.FullName
            };
            return dto;
        }

        // POST: api/books
        // 2. Zmień akcję Create:
        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto input)
        {
            // mapowanie DTO -> encja
            var book = new Book
            {
                Title = input.Title,
                ISBN = input.ISBN,
                AuthorId = input.AuthorId,
                Loans = new List<Loan>()   // inicjalizacja pustych wypożyczeń
            };

            await _uow.Books.AddAsync(book);
            await _uow.SaveChangesAsync();

            // ponownie pobieramy z autorem, by zwrócić BookDto
            var created = await _uow.Books
                .Query()
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == book.Id);

            var dto = new BookDto
            {
                Id = created!.Id,
                Title = created.Title,
                ISBN = created.ISBN,
                AuthorName = created.Author.FullName
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = dto.Id },
                dto
            );
        }


        // PUT: api/books/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book input)
        {
            if (id != input.Id)
                return BadRequest();

            var existing = await _uow.Books.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // Tylko aktualizujemy wymagane pola
            existing.Title = input.Title;
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
