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
    public class LoansController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public LoansController(IUnitOfWork uow) => _uow = uow;

        [HttpGet]
        public async Task<IEnumerable<Loan>> GetAll() =>
            await _uow.Loans.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Loan>> GetById(int id)
        {
            var loan = await _uow.Loans.GetByIdAsync(id);
            return loan is null ? NotFound() : Ok(loan);
        }

        [HttpPost, Authorize(Roles = "Admin,Member")]
        public async Task<ActionResult> Create(Loan loan)
        {
            await _uow.Loans.AddAsync(loan);
            await _uow.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, Loan loan)
        {
            if (id != loan.Id) return BadRequest();
            _uow.Loans.Update(loan);
            await _uow.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var loan = await _uow.Loans.GetByIdAsync(id);
            if (loan is null) return NotFound();
            _uow.Loans.Delete(loan);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
