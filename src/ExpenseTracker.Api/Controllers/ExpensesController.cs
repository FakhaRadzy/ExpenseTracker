using ExpenseTracker.Application.Expenses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracker.Application.Common.Models;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Authorize]             // Every endpoint in this controller now needs a valid token
    [Route("api/[controller]")]
    public class ExpensesController(IExpenseService expenseService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<ExpenseResponse>>> GetAll([FromQuery]ExpenseQuery query, CancellationToken ct)
        {
            return Ok(await expenseService.GetAllAsync(query, ct));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ExpenseResponse>> GetId(Guid id, CancellationToken ct)
        {
            var expense = await expenseService.GetIdAsync(id, ct);
            return expense is null ? NotFound() : Ok(expense);
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseResponse>> Create(ExpenseRequest request, CancellationToken ct)
        {
            var expense = await expenseService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetId), new { id = expense.Id }, expense);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ExpenseResponse>> Update(Guid id, ExpenseRequest request, CancellationToken ct)
        {
            var expense = await expenseService.UpdateAsync(id, request, ct);
            return expense is null ? NotFound() : Ok(expense);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            return await expenseService.DeleteAsync(id, ct) ? NoContent() : NotFound();
        }
    }
}
