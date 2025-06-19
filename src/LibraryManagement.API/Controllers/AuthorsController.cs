using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public AuthorsController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IEnumerable<Author>> GetAll() =>
            await _uow.Authors.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            var author = await _uow.Authors.GetByIdAsync(id);
            return author is null ? NotFound() : Ok(author);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(Author author)
        {
            await _uow.Authors.AddAsync(author);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, author);
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, Author author)
        {
            if (id != author.Id) return BadRequest();
            _uow.Authors.Update(author);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var author = await _uow.Authors.GetByIdAsync(id);
            if (author is null) return NotFound();
            _uow.Authors.Delete(author);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
