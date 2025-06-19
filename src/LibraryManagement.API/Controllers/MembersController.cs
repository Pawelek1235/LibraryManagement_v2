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
    public class MembersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public MembersController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IEnumerable<Member>> GetAll() =>
            await _uow.Members.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetById(int id)
        {
            var member = await _uow.Members.GetByIdAsync(id);
            return member is null ? NotFound() : Ok(member);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(Member member)
        {
            await _uow.Members.AddAsync(member);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, Member member)
        {
            if (id != member.Id) return BadRequest();
            _uow.Members.Update(member);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var member = await _uow.Members.GetByIdAsync(id);
            if (member is null) return NotFound();
            _uow.Members.Delete(member);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
